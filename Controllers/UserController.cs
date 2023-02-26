using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private ApplicationContext _context;
        private UserService _userService;

        public UserController(ApplicationContext context, UserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [HttpGet]
        [Authorize]
        public ActionResult<User> Get() => Ok(_userService.GetUser());
    }
}