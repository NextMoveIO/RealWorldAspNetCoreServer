using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealWordServer.Helpers;
using RealWordServer.Models;

namespace RealWordServer.Controllers
{
    public class AuthTokenDto
    {
        public string Token { get; set; }
    }
    public class RegisterUserDto
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }

    public class LoginUserDto : RegisterUserDto
    { }

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
    [EnableCors("RealWorldServerCorsPolicy")]
    public class AccountController : ApiControllerBase
    {
        public AccountController(BloggingContext context) : base(context)
        { }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public ActionResult<AuthTokenDto> Register(RegisterUserDto registerUserDto)
        {
            var emailAddress = registerUserDto.EmailAddress.ToLowerInvariant();
            var saltHash = new CryptoKey().HashPassword(registerUserDto.Password);
            var user = new User { EmailAddress = emailAddress, PasswordHash = saltHash.Hash, PasswordSalt = saltHash.Salt };

            Context.Users.Add(user);
            Context.SaveChanges();

            // Login
            var token = Guid.NewGuid().ToString("N");
            var tokenRecord = new Token { UserId = user.UserId, Secret = token, Expires = DateTime.UtcNow.AddHours(3) };
            Context.Tokens.Add(tokenRecord);
            Context.SaveChanges();

            return new AuthTokenDto { Token = token };
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public ActionResult<AuthTokenDto> Login(LoginUserDto loginUserDto)
        {
            var token = Guid.NewGuid().ToString("N");
            var emailAddress = loginUserDto.EmailAddress.ToLowerInvariant();
            var user = Context.Users.Where(_ => _.EmailAddress == emailAddress).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }

            if (!new CryptoKey().Verify(loginUserDto.Password, new SaltHash { Hash = user.PasswordHash, Salt = user.PasswordSalt}))
            {
                throw new Exception("Password mismatch");
            }

            // Login
            var tokenRecord = new Token { UserId = user.UserId, Secret = token, Expires = DateTime.UtcNow.AddHours(3) };
            Context.Tokens.Add(tokenRecord);
            Context.SaveChanges();

            return new AuthTokenDto { Token = token };
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