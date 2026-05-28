using Demo1.DTO;
using Demo1.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Demo1.Controllers {
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase {
        private readonly List<User> users = new(){
            new User { Username = "admin", Password = "1234", FirstName = "John", LastName = "Doe", IsAdmin = true },
            new User { Username = "bob", Password = "5678", FirstName = "Sponge", LastName = "Bob", IsAdmin = false },
        };

        [HttpPost("login")]
        public ActionResult<string> Login(LoginBodyDTO loginBody) {
            var user = ValidateUser(loginBody.Username, loginBody.Password);

            if (user == null) {
                return Unauthorized();
            }

            return Ok("OK");
        }

        private User? ValidateUser(string username, string password) {
            return users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }
    }
}
