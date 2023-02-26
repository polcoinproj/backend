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
    public class TransactionController : ControllerBase
    {
        private UserService _userService;
        private ApplicationContext _context;

        public TransactionController(UserService userService, ApplicationContext context)
        {
            _userService = userService;
            _context = context;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Make(TransactionMake req)
        {
            User from = _userService.GetUser();
            if (_userService.GetRole() == UserRoles.User)
            {
                if (from.Balance < req.Amount)
                {
                    return BadRequest("coinsNotEnough");
                }

                from.Balance -= (int)req.Amount;
            }

            Transaction t = new Transaction
            {
                From = from,
                Amount = (int)req.Amount,
                Comment = req.Comment
            };

            if (req.To != 0)
            { // to user
                User? to = _context.Users.FirstOrDefault((x) => x.Id == req.To);
                if (to == null)
                {
                    return BadRequest("targetUserNotFound");
                }

                to!.Balance += (int)req.Amount;

                t.To = to;
            }
            else
            { // to service
                t.Service = req.Service;
            }

            await _context.AddAsync(t);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<Transaction[]>> Get() => Ok(await _context.Transactions.Where((x) => x.From == _userService.GetUser()).ToListAsync());
    }
}