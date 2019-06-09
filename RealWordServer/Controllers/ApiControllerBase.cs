using Microsoft.AspNetCore.Mvc;
using RealWordServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RealWordServer.Controllers
{
    public class ApiControllerBase : ControllerBase
    {
        protected readonly BloggingContext Context;
        public ApiControllerBase(BloggingContext context)
        {
            Context = context;
        }

        protected UserDto GetUser()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;

            var id = claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier).Value;

            return new UserDto
            {
                Id = int.Parse(id),
                EmailAddress = claims.FirstOrDefault(_ => _.Type == ClaimTypes.Email).Value,
                FirstName = claims.FirstOrDefault(_ => _.Type == ClaimTypes.GivenName).Value,
                LastName = claims.FirstOrDefault(_ => _.Type == ClaimTypes.Surname).Value,
            };
        }
    }
}
