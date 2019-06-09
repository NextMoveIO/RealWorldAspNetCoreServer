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
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly BloggingContext Context;

        public AccountController(BloggingContext context)
        {
            Context = context;
        }

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
        [Authorize(AuthenticationSchemes = "BearerAuthentication")]
        [Route("me")]
        public ActionResult<UserDto> Me()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;

            return new UserDto
            {
                Id = claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier).Value,
                EmailAddress = claims.FirstOrDefault(_ => _.Type == ClaimTypes.Email).Value,
                FirstName = claims.FirstOrDefault(_ => _.Type == ClaimTypes.GivenName).Value,
                LastName = claims.FirstOrDefault(_ => _.Type == ClaimTypes.Surname).Value,
            };
        }

        private string HashPassword(string password)
        {
            return password;
        }
    }
}