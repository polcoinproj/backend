using backend.Models;
using backend.Models.Dto;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AchievementController : ControllerBase
    {
        private ApplicationContext _context { get; }
        private IConfiguration _configuration { get; }
        private FileService<AchievementController> _fileService { get; }
        private UserService _userService { get; }

        public AchievementController(ApplicationContext context, IConfiguration configuration, FileService<AchievementController> fileService, UserService userService)
        {
            _context = context;
            _configuration = configuration;
            _fileService = fileService;
            _userService = userService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<int>> Create(AchievementCreate req)
        {
            Achievement achievement = new Achievement
            {
                User = _userService.GetUser(),
                Status = Status.InProcess,
                Comment = req.Comment,
            };

            await _context.AddAsync(achievement);
            await _context.SaveChangesAsync();

            return Ok(achievement.Id);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> AddAttachments(int id)
        {
            Achievement? achievement = await _context.Achievements.FirstOrDefaultAsync((x) => x.Id == id && x.User == _userService.GetUser());
            if (achievement == null)
            {
                return BadRequest("achievementNotFound");
            }

            foreach (IFormFile file in Request.Form.Files)
            {
                string uri = await _fileService.Upload(file);
                if (uri == "")
                {
                    return BadRequest();
                }

                achievement.Attachments = achievement.Attachments.Append(uri).ToArray();
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("byStatus/{status}")]
        [Authorize(Roles = UserRoles.Administration)]
        public async Task<ActionResult<Achievement[]>> Get(Status status)
            => Ok(await _context.Achievements.Where((x) => x.Status == status && x.User.School == _userService.GetUser().School)
            .Include((x) => x.User)
            .Select((x) => new
            {
                Id = x.Id,
                Status = x.Status,
                Comment = x.Comment,
                Attachments = x.Attachments,
                User = new
                {
                    Id = x.User.Id,
                    FirstName = x.User.FirstName,
                    LastName = x.User.LastName,
                    School = x.User.School,
                    Grade = x.User.Grade
                }
            })
            .ToListAsync());

        [HttpPatch("{id}")]
        [Authorize(Roles = UserRoles.Administration)]
        public async Task<ActionResult> Approve(int id)
        {
            Achievement? achievement = await _context.Achievements.FirstOrDefaultAsync((x) => x.Id == id && x.User.School == _userService.GetUser().School);
            if (achievement == null)
            {
                return BadRequest("achievementNotFound");
            }

            if (achievement.Status != Status.InProcess)
            {
                return BadRequest("alreadyProcessed");
            }

            achievement.Status = Status.Approved;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<Achievement[]>> GetUser() => Ok(await _context.Achievements.Where((x) => x.User == _userService.GetUser()).ToListAsync());
    }
}