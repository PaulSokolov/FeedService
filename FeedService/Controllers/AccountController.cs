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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using FeedService.Infrastructure;

namespace FeedService.Controllers
{
    [Produces("application/json")]
    [Route("api/Account")]
    public class AccountController : Controller
    {
        IFeedServiceUoW _db;
        ILogger _logger;

        public AccountController(IFeedServiceUoW db, ILogger<AccountController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpPost("/register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorObject(ErrorMessages.BAD_REQUEST_ERROR));
            }

            User tempUser = _db.Users.GetAll().SingleOrDefault(u => u.Login == user.Login);

            if(tempUser != null)
            {
                return BadRequest(new ErrorObject(ErrorMessages.USER_ALREADY_EXISTS_ERROR));
            }

            if(user.Password.Length < 6)
            {
                return BadRequest(new ErrorObject(ErrorMessages.WRONG_PASSWORD_LENGTH));
            }

            try
            {
                await _db.Users.AddAsync(user);
                await _db.Users.SaveAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(new EventId(1), ex, "{0} ERROR: {1}", DateTime.Now, ex.Message);
                return BadRequest(new { Error = "Server error. Try later." });
            }

            return Ok(new SuccessObject($"User {user.Login} " + SuccessMessages.REGISTRATION_SUCCESS));
        }

        [HttpPost("/token")]
        public async Task<IActionResult> Token()
        {
            StringValues username = default(StringValues);
            StringValues password = default(StringValues);
            ClaimsIdentity identity = null;

            try
            {
                username = Request.Form["username"];
                password = Request.Form["password"];

                identity = GetIdentity(username, password);
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(1), ex, "{0} ERROR: {1}", DateTime.Now, ex.Message);
                return BadRequest(new { Error = "Server error. Try later." });
            }

            if (identity == null)
            {
                return Ok(new SuccessObject(ErrorMessages.INVALID_USERNAME_OR_PASSWORD_ERROR));
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


            return Ok(new SuccessObject { Result = response });
        }

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            User person = _db.Users.GetAll().FirstOrDefault(x => x.Login == username && x.Password == password);
         

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