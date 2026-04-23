using Demo1.DataStores;
using Demo1.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Demo1.Controllers {
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase {

        [HttpGet]
        public IEnumerable<CityDTO> GetCities() {
            return CitiesDataStore.Current;
        }

        [HttpGet("{id}")]
        public ActionResult<CityDTO> GetCity(int id) {
            var city = CitiesDataStore.
                Current.
                FirstOrDefault(c => c.ID == id);

            if (city == null) {
                return NotFound();
            }

            return city;
        }

        [HttpGet("problem/{id}")]
        public ActionResult MakeAProblem(int id) {

            //var problem = new ProblemDetails() {
            //    Title = "This is a problem",
            //    Detail = "This is a detailed description of the problem",
            //    Status = 301                
            //};

            //return BadRequest(problem);

            return Problem($"An error with id: {id}", "An Instance", 456);
        }
    }   
}
