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
    public class ContactController : ControllerBase
    {
        private ApplicationContext _context { get; }
        private UserService _userService { get; }

        public ContactController(ApplicationContext context, UserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Add(ContactCreateDto req)
        {
            User client = _userService.GetUser();

            User? u = _context.Users.FirstOrDefault((x) => x.Id == req.Target);
            if (u == null)
            {
                return BadRequest("userNotFound");
            }

            if (u == client)
            {
                return BadRequest("cannotAddSelf");
            }

            Contact? c = _context.Contacts.FirstOrDefault((x) => x.Owner == client && x.Target == u);
            if (c != null)
            {
                return BadRequest("contactExists");
            }

            await _context.AddAsync(new Contact
            {
                Owner = client,
                Target = u,
            });
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<Contact[]>> Get() => Ok(await _context.Contacts.Where((x) => x.Owner == _userService.GetUser())
                .Include((x) => x.Target).Select(
                    (x) => new
                    {
                        Id = x.Target.Id,
                        FirstName = x.Target.FirstName,
                        LastName = x.Target.LastName,
                        Grade = x.Target.Grade,
                        School = x.Target.School.Name
                    }).ToListAsync());


        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> Delete(int id)
        {
            Contact? c = _context.Contacts.FirstOrDefault((x) => x.Owner.Id == _userService.GetId() && x.Target.Id == id);
            if (c == null)
            {
                return BadRequest("contactNotFound");
            }

            _context.Remove(c!);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}