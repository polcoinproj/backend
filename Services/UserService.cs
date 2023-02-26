using System.Security.Claims;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class UserService
    {
        IHttpContextAccessor httpContext;
        ApplicationContext context;

        public UserService(IHttpContextAccessor httpContext, ApplicationContext context)
        {
            this.httpContext = httpContext;
            this.context = context;
        }

        public User GetUser()
        {
            Claim? claim = httpContext.HttpContext!.User.FindFirst(ClaimTypes.Name);
            if (claim == null)
            {
                httpContext.HttpContext!.Abort();

                return new User { };
            }

            int id = int.Parse(claim.Value);

            User? user = context.Users.Include((x) => x.School).FirstOrDefault((x) => x.Id == id);
            if (user == null)
            {
                httpContext.HttpContext!.Abort();

                return new User { };
            }

            return user;
        }

        public string GetRole()
        {
            Claim? claim = httpContext.HttpContext!.User.FindFirst(ClaimTypes.Role);
            if (claim == null)
            {
                return UserRoles.User;
            }

            return claim.Value;
        }

        public int GetId()
        {
            Claim? claim = httpContext.HttpContext!.User.FindFirst(ClaimTypes.Name);
            if (claim == null)
            {
                httpContext.HttpContext!.Abort();

                return 0;
            }

            return int.Parse(claim.Value);
        }
    }
}