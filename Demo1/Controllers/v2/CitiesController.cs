using Asp.Versioning;
using AutoMapper;
using Demo1.DataStores;
using Demo1.DbContexts;
using Demo1.DTO.v2;
using Demo1.Services;
using Demo1.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog.Context;


namespace Demo1.Controllers.v2 {
    [ApiController]
    // [Authorize]
    [Route("api/v{version:apiVersion}/cities")]
    [ApiVersion(2)]
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
        public async Task<ActionResult<IEnumerable<CityWithoutLandmarksDTO>>> GetCities() {
            return Ok(new List<CityWithoutLandmarksDTO>() {
                new CityWithoutLandmarksDTO() {
                    Sku = 111,
                    Name = $"City 111",
                    Description = $"Description for city 111"
                }
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CityWithoutLandmarksDTO>> GetCity(int id) {
            return Ok(new CityWithoutLandmarksDTO() {
                Sku = id,
                Name = $"City {id}",
                Description = $"Description for city {id}"
            });
        }
    }
}