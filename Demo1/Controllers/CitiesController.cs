using Demo1.DataStores;
using Demo1.DTO;
using Demo1.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog.Context;

namespace Demo1.Controllers {
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase {
        private readonly ILogger<CitiesController> _logger;
        private readonly IEmailService _email;
        public CitiesController(ILogger<CitiesController> logger, IEmailService email) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _email = email ?? throw new ArgumentNullException(nameof(email));
        }


        [HttpGet]
        public IEnumerable<CityDTO> GetCities() {
            _logger.LogInformation("No Property here");
            using (LogContext.PushProperty("Simon", Guid.NewGuid())) { 
                _logger.LogInformation("Getting all cities");
                _logger.LogInformation("Returned {CityCount} cities", CitiesDataStore.Current.Count);
                return CitiesDataStore.Current;
            }
        }

        [HttpGet("{id}")]
        public ActionResult<CityDTO> GetCity(int id) {

            var city = CitiesDataStore.
                Current.
                FirstOrDefault(c => c.ID == id);

            if (city == null) {
                return NotFound();
            }

            _email.Send("City was accessed", $"City with id: {id} was accessed.");

            return city;
        }

        [HttpGet("problem/{id}")]
        public ActionResult MakeAProblem(int id) {

            var problem = new ProblemDetails() {
                Title = "This is a problem",
                Detail = "This is a detailed description of the problem",
                //Status = 301
            };

            return BadRequest(problem);

            //return Problem($"An error with id: {id}", "An Instance", 456);
        }
    }   
}