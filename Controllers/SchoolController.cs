using backend.Models;
using backend.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchoolController : ControllerBase
    {
        private ApplicationContext _context;

        public SchoolController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<School[]> Get()
        {
            return Ok(_context.Schools);
        }

        [Authorize(Roles = UserRoles.SuperUser)]
        [HttpPost]
        public async Task<ActionResult> Create(SchoolCreate req)
        {
            School sc = new School
            {
                Name = req.Name,
            };

            await _context.AddAsync(sc);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}