using Demo1.DataStores;
using Demo1.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Demo1.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase {

        [HttpGet]
        public IEnumerable<CityDTO> GetCities() {
            return CitiesDataStore.Current;
        }
    }   
}
