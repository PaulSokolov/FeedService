using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using FeedService.DbModels;
using FeedService.DbModels.Interfaces;

namespace FeedService.Controllers
{
    [Produces("application/json")]
    [Route("api/Account")]
    public class AccountController : Controller
    {
        IRepository<User> _userRepository;

        public AccountController(IRepository<User> userRepository):base()
        {
            _userRepository = userRepository;
        }

        [HttpPost("/register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Error = "Invalid request parameters"});
            }

            User tempUser = _userRepository.GetAll().SingleOrDefault(u => u.Login == user.Login);
            if(tempUser != null)
            {
                return BadRequest(new { Error = "User with such login already exists" });
            }
            if(user.Password.Length < 6)
            {
                return BadRequest( new { Error = "Password should be at least 6 symbols" });
            }
            await _userRepository.AddAsync(user);
            await _userRepository.SaveAsync();

            return Ok(new { Success = $"User {user.Login} registered successfully" });
        }

        [HttpPost("/token")]
        public async Task<IActionResult> Token()
        {
            var username = Request.Form["username"];
            var password = Request.Form["password"];

            var identity = GetIdentity(username, password);
            if (identity == null)
            {
                return BadRequest(new { Error = "Invalid username or password." });
            }

            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };


            return Ok(response);
        }

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            User person = _userRepository.GetAll().FirstOrDefault(x => x.Login == username && x.Password == password);
            if (person != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.Login),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, person.Role)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
            
            return null;
        }

    }
}