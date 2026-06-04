using Asp.Versioning;
using Demo1.DTO;
using Demo1.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Demo1.Controllers {

    /// <summary>
    /// Controller for handling authentication and authorization
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/auth")]
    [ApiVersion(2)]
    [ApiVersion(1)]
    public class AuthController : ControllerBase {
        private readonly List<User> users = new(){
            new User { Username = "admin", Password = "1234", FirstName = "John", LastName = "Doe", IsAdmin = true },
            new User { Username = "bob", Password = "5678", FirstName = "Sponge", LastName = "Bob", IsAdmin = false },
        };

        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration) {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }


        /// <summary>
        /// This is a silly function to demonstrate versioning.
        /// </summary>
        /// <response code="404">If the item is not found SIMON!!</response>
        /// <response code="401">If the user is not authorized BOB!!!</response>
        /// <returns>always returns the same thing</returns>
        [HttpGet("something")]
        [MapToApiVersion(1)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<string> GetSomething() {
            
            
            
            return Ok("This is something!!!");
        }

        /// <summary>
        /// This is a new version of the same function to demonstrate versioning.
        /// </summary>
        /// <param name="name">this is your name</param>
        /// <returns>the same string with your name at the end</returns>
        [HttpGet("something")]
        [MapToApiVersion(2)]
        public ActionResult<string> GetSomething2(string name) {
            return Ok($"This something is better!!! Hey {name}");
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
                new Claim(JwtRegisteredClaimNames.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new Claim(JwtRegisteredClaimNames.Iss, _configuration["Authentication:Issuer"]
                    ?? throw new ArgumentNullException("Issuer not found in settings")),
                new Claim(JwtRegisteredClaimNames.Aud, _configuration["Authentication:Audience"]
                    ?? throw new ArgumentNullException("Audience not found in settings")),
                new Claim("IsAdmin", user.IsAdmin ? "Admin" : "User"),
                // new Claim("BulkEnabled", DateTime.Now.DayOfWeek == DayOfWeek.Tuesday ? "True" : "False")
                // new Claim("Password", user.Password) // dont do this !!!!!
            };

            // create token
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                notBefore: DateTime.Now,
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
