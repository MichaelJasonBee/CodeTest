using EVoucherAndStoreAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EVoucherAndStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;

        public LoginController(IConfiguration config)
        {
            _config = config;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] UserLogin userLogin)
        {
            var user = Authenticate(userLogin);

            if (user != null) 
            {
                var token = Generate(user);
                return Ok(new { AccessToken = token, PhoneNumber = user.MobilePhone, Role = user.Role });
            }

            return NotFound("User not found");
        }

        [HttpPost]
        [Route("Refresh")]
        public IActionResult Refresh([FromBody] Token request)
        {
            if (request is null)
                return BadRequest();

            if (string.IsNullOrEmpty(request.AccessToken))
                return BadRequest();

            var claimsPrincipal = GetClaimsPrincipal(request);

            if (claimsPrincipal is null)
                return NotFound("Invalid token");

            var expirySetting = Convert.ToInt32(_config["Jwt:ExpiryTimeInSecond"].ToString());
            var expiryDateInClaims = long.Parse(claimsPrincipal.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiryDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateInClaims);
            
            if (expiryDate > DateTime.UtcNow)
                return BadRequest("This token hasn't expired yet");

            
            // Check token in db
            //here
            //

            var userModel = new UserModel
            {
                Username = claimsPrincipal.Claims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value,
                EmailAddress = claimsPrincipal.Claims.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value,
                GivenName = claimsPrincipal.Claims.FirstOrDefault(o => o.Type == ClaimTypes.GivenName)?.Value,
                Surname = claimsPrincipal.Claims.FirstOrDefault(o => o.Type == ClaimTypes.Surname)?.Value,
                Role = claimsPrincipal.Claims.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value
            };

            var token = Generate(userModel);

            return Ok(new Token() { AccessToken = token });
        }

        private ClaimsPrincipal GetClaimsPrincipal(Token token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            
            try
            {
                var tokenvalidationParam = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),                    
                };
                var principal = tokenHandler.ValidateToken(token.AccessToken.ToString(), tokenvalidationParam, out var validatedToken);
                if (!IsJwtwithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }
                return principal;

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private bool IsJwtwithValidSecurityAlgorithm(SecurityToken securityToken )
        {
            return (securityToken is JwtSecurityToken jwtSecurityToken) &&
               jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
        }

        private string Generate(UserModel user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Email, user.EmailAddress),
                new Claim(ClaimTypes.GivenName, user.GivenName),
                new Claim(ClaimTypes.Surname, user.Surname),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddSeconds(Convert.ToInt32(_config["Jwt:ExpiryTimeInSecond"].ToString())),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private UserModel Authenticate(UserLogin userLogin)
        {
            var currentUser = UserConstant.Users.FirstOrDefault(o => o.Username.ToLower() == userLogin.Username.ToLower() && o.Password == userLogin.Password);

            if (currentUser != null)
            {
                return currentUser;
            }

            return null;
        }
    }
}
