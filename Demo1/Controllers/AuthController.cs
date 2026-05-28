using Demo1.DTO;
using Demo1.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Demo1.Controllers {
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase {
        private readonly List<User> users = new(){
            new User { Username = "admin", Password = "1234", FirstName = "John", LastName = "Doe", IsAdmin = true },
            new User { Username = "bob", Password = "5678", FirstName = "Sponge", LastName = "Bob", IsAdmin = false },
        };

        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration) {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }


        [HttpPost("login")]
        public ActionResult<string> Login(LoginBodyDTO loginBody) {
            var user = ValidateUser(loginBody.Username, loginBody.Password);

            if (user == null) {
                return Unauthorized();
            }

            // get secret key from appsettings or from environment variable
            var configKey = _configuration["Authentication:SecretKey"] ?? 
                throw new ArgumentNullException("Secret key not found in settings");

            // create symmetric security key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configKey));

            // create signing credentials
            var signingCreds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // create claims
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User"),
                new Claim("Password", user.Password)
            };

            // create token
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: signingCreds
            );

            // return token as string
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);


            return Ok(tokenString);
        }

        private User? ValidateUser(string username, string password) {
            return users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }
    }
}
