using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealWordServer.Models;

namespace RealWordServer.Controllers
{
    public class RegisterUserDto
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "BearerAuthentication")]
    public class AccountController : ApiControllerBase
    {
        public AccountController(BloggingContext context) : base(context)
        { }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public ActionResult<string> Register(RegisterUserDto registerUserDto)
        {
            var token = Guid.NewGuid().ToString("N");
            var user = new User { EmailAddress = registerUserDto.EmailAddress, PasswordHash = HashPassword(registerUserDto.Password) };
            Context.Users.Add(user);
            Context.SaveChanges();

            // Login
            var tokenRecord = new Token { UserId = user.UserId, Secret = token, Expires = DateTime.UtcNow.AddHours(3) };
            Context.Tokens.Add(tokenRecord);
            Context.SaveChanges();

            return token;
        }

        [HttpGet]
        [Route("me")]
        public ActionResult<UserDto> Me()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;

            return GetUser();
        }

        private string HashPassword(string password)
        {
            return password;
        }
    }
}