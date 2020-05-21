using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JWTAuthDemo.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace JWTAuthDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;

        public LoginController(IConfiguration configuration)
        {
            _config = configuration;
        }

        public IActionResult Login(string username, string pass)
        {
            User login = new User();

            login.UserName = username;
            login.Password = pass;

            IActionResult response = Unauthorized();

            var user = this.AuthenticateUser(login);

            if(user != null)
            {
                var tokenStr = GenerateJSONWebToken(user);

                response = Ok(new { token = tokenStr });
            }

            return response;
        }

       
        private User AuthenticateUser(User login)
        {
            User user = null;

            if(login.UserName == "rzamperlini" && login.Password == "123")
            {
                user = new User { UserName = login.UserName, EmailAddress = "rzamperlini@gmail.com", Password = login.Password };
            }

            return user;
        }

        private string GenerateJSONWebToken(User userinfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userinfo.UserName),
                new Claim(JwtRegisteredClaimNames.Email, userinfo.EmailAddress),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer:_config["Jwt:Issuer"],
                audience:_config["Jwt:Issuer"],
                claims,
                DateTime.Now,
                DateTime.Now.AddMinutes(120),
                credentials
                );

            var encodeToken = new JwtSecurityTokenHandler().WriteToken(token);

            return encodeToken;
        }

        [Authorize]
        [HttpPost("Post")]
        public string Post()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claims = identity.Claims.ToList();
            var userName = claims[0].Value;
            return "Welcome "+userName;
        }

        [Authorize]
        [HttpGet("GetValue")]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[]{ "value1", "value2", "value3"};
        }

    }
}