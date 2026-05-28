using AutoMapper;
using Demo1.DataStores;
using Demo1.DbContexts;
using Demo1.DTO;
using Demo1.Services;
using Demo1.Services.Repositories;
using Microsoft.AspNetCore.Mvc;
using Serilog.Context;

namespace Demo1.Controllers {
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase {
        private readonly ILogger<CitiesController> _logger;
        private readonly IEmailService _email;
        private readonly ICityRepository _cityRepository;
        private readonly IMapper _mapper;

        public CitiesController(ILogger<CitiesController> logger, IEmailService email, ICityRepository cityRepository, IMapper mapper) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _email = email ?? throw new ArgumentNullException(nameof(email));
            _cityRepository = cityRepository ?? throw new ArgumentNullException(nameof(cityRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutLandmarksDTO>>> GetCities(
            string? name,
            string? search,
            int? pageNumber,
            int? pageSize
            ) {
            #region emails
            //_logger.LogInformation("No Property here");
            //using (LogContext.PushProperty("Simon", Guid.NewGuid())) {
            //    _email.Send("Getting all cities", "Getting all cities was called.");

            //    _logger.LogInformation("Getting all cities");
            //    _logger.LogInformation("Returned {CityCount} cities", CitiesDataStore.Current.Count);

            //    _email.Send("All cities were accessed", $"All cities were accessed at {DateTime.UtcNow}.");

            //    return CitiesDataStore.Current;
            //}


            //var cities = new List<CityWithoutLandmarksDTO>();

            //foreach (var city in await _cityRepository.GetCitiesAsync()) {
            //    cities.Add(new CityWithoutLandmarksDTO() {
            //        ID = city.Id,
            //        Name = city.Name,
            //        Description = city.Description
            //    });
            //}
            #endregion

            var (cities, metadata) = await _cityRepository.GetCitiesAsync(name, search, pageNumber, pageSize);

            //Response.Headers.Append("X-Pagination", System.Text.Json.JsonSerializer.Serialize(metadata));
            Response.Headers.Append("X-Pagination-PageSize", metadata.PageSize.ToString());
            Response.Headers.Append("X-Pagination-TotalItemCount", metadata.TotalItemCount.ToString());
            Response.Headers.Append("X-Pagination-CurrentPage", metadata.CurrentPage.ToString());
            Response.Headers.Append("X-Pagination-TotalPageCount", metadata.TotalPageCount.ToString());

            return Ok(_mapper.Map<List<CityWithoutLandmarksDTO>>(cities));

            // this is ok, but not ideal, we want to return the metadata in the headers, not the body
            //return Ok(
            //    new {
            //        data = _mapper.Map<List<CityWithoutLandmarksDTO>>(cities),
            //        meta = metadata
            //    }
            //   );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(int id, bool includeLandmarks = false) {

            var city = await _cityRepository.GetCityAsync(id, includeLandmarks);

            if (city == null) {
                return NotFound();
            }

            if (includeLandmarks) {
                return Ok(_mapper.Map<CityDTO>(city));
            }
            return Ok(_mapper.Map<CityWithoutLandmarksDTO>(city));

            //CityDTO cityDto = new CityDTO() {
            //    ID = city.Id,
            //    Name = city.Name,
            //    Description = city.Description,
            //    LandMarks = city.LandMarks.Select(l => new LandMarkDTO() {
            //        ID = l.Id,
            //        Name = l.Name,
            //        Description = l.Description
            //    })
            //};

            //return Ok(cityDto);

            //var city = CitiesDataStore.
            //    Current.
            //    FirstOrDefault(c => c.ID == id);

            //if (city == null) {
            //    return NotFound();
            //}

            //_email.Send("City was accessed", $"City with id: {id} was accessed.");

            //return city;
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