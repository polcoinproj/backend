using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Models.Dto;
using backend.Models;
using backend.Services;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Security.Claims;
using System.Net;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private ApplicationContext _context { get; }
        private EmailChecker _emailChecker { get; }
        private IConfiguration _cfg { get; }

        private SmtpClient _smtpClient { get; }
        private string _from { get; }

        private byte[] _privateKey { get; }

        public AuthController(ApplicationContext context, EmailChecker emailChecker, IConfiguration cfg)
        {
            _context = context;
            _emailChecker = emailChecker;
            _cfg = cfg;

            IConfigurationSection emailData = cfg.GetSection("Email");
            _from = emailData.GetValue<string>("From")!;

            _smtpClient = new SmtpClient(emailData.GetValue<string>("Host"))
            {
                Port = emailData.GetValue<int>("Port"),
                Credentials = new NetworkCredential(_from, emailData.GetValue<string>("Password")),
                EnableSsl = true,
            };

            _privateKey = System.IO.File.ReadAllBytes(cfg.GetSection("Jwt").GetValue<string>("Key")!);
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(AuthRegister req)
        {
            if (_context.Users.Any((x) => x.Email == req.Email))
            {
                return BadRequest("emailUsed");
            }

            if (_emailChecker.IsBlacklisted(new MailAddress(req.Email).Host))
            {
                return BadRequest("badEmail");
            }

            CreatePasswordHash(req.Password, out byte[] hash, out byte[] salt);

            School? sc = _context.Schools.FirstOrDefault((x) => x.Id == req.School);
            if (sc == null)
            {
                return BadRequest("noSuchSchool");
            }

            User user = new User
            {
                PasswordHash = hash,
                PasswordSalt = salt,

                FirstName = req.FirstName,
                LastName = req.LastName,
                Email = req.Email,
                Grade = req.Grade,

                School = sc,
                Verified = false,
                Role = UserRoles.User,
            };

            int code = new Random().Next(100000, 999999);

            VerifyEntry verify = new VerifyEntry
            {
                Code = code,
                User = user,
            };

            await _context.AddAsync(user);
            await _context.AddAsync(verify);
            await _context.SaveChangesAsync();

            await _smtpClient.SendMailAsync(new MailMessage(_from, req.Email)
            {
                Subject = "Регистрация в PolCoin",
                Body = $"Ссылка для подтверждения вашего почтового ящика - {_cfg.GetValue<string>("currentHost")}/api/auth/verifyEmail/{user.Id}/{code}",
                BodyEncoding = System.Text.Encoding.UTF8,
            });

            return Ok();
        }

        [HttpGet("verifyEmail/{id}/{code}")]
        public async Task<ActionResult> Verify(int id, int code)
        {
            VerifyEntry data;

            try
            {
                data = _context.VerifyEntries.Include((x) => x.User).First((x) => x.Id == id && x.Code == code);
            }
            catch (InvalidOperationException)
            {
                return BadRequest("verifyEntryNotFound");
            }

            _context.Remove(data);
            data.User.Verified = true;
            await _context.SaveChangesAsync();

            return Ok("Вы успешно подтвердили свой почтовый адрес");
        }

        [HttpPost("login")]
        public ActionResult Login(AuthLogin req)
        {
            User user;

            try
            {
                user = _context.Users.First((x) => x.Email == req.Email && x.Verified);
            }
            catch (InvalidOperationException)
            {
                return BadRequest("userNotFound");
            }

            if (!VerifyPasswordHash(req.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("invalidPassword");
            }

            return Ok(CreateToken(user));
        }

        private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            HMACSHA512 hmac = new HMACSHA512();

            salt = hmac.Key;
            hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPasswordHash(string password, byte[] hash, byte[] salt)
            => new HMACSHA512(salt).ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)).SequenceEqual(hash);

        private string CreateToken(User user)
        {
            ClaimsIdentity claims = new ClaimsIdentity(new Claim[]{
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
            });

            IConfigurationSection jwtConfig = _cfg.GetSection("Jwt");

            SecurityTokenDescriptor desc = new SecurityTokenDescriptor
            {
                Subject = claims,
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_privateKey), SecurityAlgorithms.HmacSha256Signature),
                Issuer = jwtConfig.GetValue<string>("Issuer"),
                Audience = jwtConfig.GetValue<string>("Audience"),
            };

            JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();
            SecurityToken tok = jwtHandler.CreateToken(desc);

            return jwtHandler.WriteToken(tok);
        }
    }
}