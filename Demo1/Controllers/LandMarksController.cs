using AutoMapper;
using Demo1.DataStores;
using Demo1.DTO;
using Demo1.Entities;
using Demo1.Services;
using Demo1.Services.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Demo1.Controllers {
    [ApiController]
    [Route("api/cities/{cityID}/landmarks")]
    public class LandMarksController : Controller {
        private readonly ILogger<LandMarksController> _logger;
        private readonly IEmailService _email;
        private readonly IMapper _mapper;
        private readonly ICityRepository _cityRepository;
        private readonly ILandMarkRepository _landMarkRepository;

        public LandMarksController(ILogger<LandMarksController> logger, 
            IEmailService email, 
            IMapper mapper, 
            ICityRepository cityRepository,
            ILandMarkRepository landMarkRepository) {

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _email = email ?? throw new ArgumentNullException(nameof(email));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _cityRepository = cityRepository ?? throw new ArgumentNullException(nameof(cityRepository));
            _landMarkRepository = landMarkRepository ?? throw new ArgumentNullException(nameof(landMarkRepository));
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<LandMarkDTO>>> GetLandMarks(int cityID) {
            ////throw new Exception("Exception ERROR!!!!");

            ////try {
            //var city = CitiesDataStore.Current.FirstOrDefault(c => c.ID == cityID);

            //if (city == null) {
            //    _logger.LogInformation($"City with id {cityID} was not found when accessing landmarks.");
            //    return NotFound();
            //    //throw new ArgumentNullException("City not found");
            //}

            //_logger.LogInformation($"Returned {city.LandMarks.Count()} landmarks for city with id {cityID}.");

            //_email.Send("Landmarks were accessed", $"Landmarks for city with id {cityID} were accessed at {DateTime.UtcNow}.");

            //return Ok(city.LandMarks);
            ////} catch (Exception ex) {
            ////    _logger.LogCritical($"Exception while getting landmarks for city with id {cityID}. {ex.Message}", ex);

            ////    return StatusCode(500, "A problem happened while handling your request.");
            ////}
            ///

            if (!await _cityRepository.ExistsAsync(cityID)) {
                return NotFound();
            }

            var landMarks = await _landMarkRepository.GetForCityAsync(cityID);

            return Ok(_mapper.Map<IEnumerable<LandMarkDTO>>(landMarks));
        }

        [HttpGet("{landMarkID}"/*, Name = "GetLandMark"*/)]
        public async Task<ActionResult<LandMarkDTO>> GetLandMark(int cityID, int landMarkID) {
            //var city = DataStores.CitiesDataStore.Current.FirstOrDefault(c => c.ID == cityID);

            //if (city == null) {
            //    return NotFound();
            //}

            //var landMark = city.LandMarks.FirstOrDefault(l => l.ID == landMarkID);

            //if (landMark == null) {
            //    return NotFound();
            //}

            //return Ok(landMark);


            if (!await _cityRepository.ExistsAsync(cityID)) {
                return NotFound();
            }

            var landMark = await _landMarkRepository.GetLandMarkAsync(cityID, landMarkID);

            if (landMark == null) {
                return NotFound();
            }

            return Ok(_mapper.Map<LandMarkDTO>(landMark));
        }

        [HttpPost]
        public async Task<ActionResult<LandMarkDTO>> AddLandMark(int cityID, LandMarkForCreateDTO newLandMark) {
            if (!await _cityRepository.ExistsAsync(cityID)) {
                return NotFound();
            }

            LandMark finalLandMark = _mapper.Map<LandMark>(newLandMark);

            await _landMarkRepository.AddLandMarkAsync(cityID, finalLandMark);
            
            return CreatedAtAction(
                nameof(GetLandMark),
                new {
                    cityID = cityID,
                    landMarkID = finalLandMark.Id
                },
                _mapper.Map<LandMarkDTO>(finalLandMark)
            );
        }

        [HttpPut("{landMarkID}")]
        public ActionResult UpdateLandMark(int cityID, int landMarkID, LandMarkForUpdateDTO updatedLandMark) {
            var city = DataStores.CitiesDataStore.Current.FirstOrDefault(c => c.ID == cityID);

            if (city == null) {
                return NotFound();
            }

            var landMarkToUpdate = city.LandMarks.FirstOrDefault(lm => lm.ID == landMarkID);

            if (landMarkToUpdate == null) {
                return NotFound();
            }

            landMarkToUpdate.Name = updatedLandMark.Name;
            landMarkToUpdate.Description = updatedLandMark.Description;

            return NoContent();
        }

        [HttpPatch("{landMarkID}")]
        public ActionResult PatchLandMark(
            int cityID,
            int landMarkID,
            JsonPatchDocument<LandMarkForUpdateDTO> patchDoc) {

            var city = DataStores.CitiesDataStore.Current.FirstOrDefault(c => c.ID == cityID);

            if (city == null) {
                return NotFound();
            }

            var landMarkToUpdate = city.LandMarks.FirstOrDefault(lm => lm.ID == landMarkID);

            if (landMarkToUpdate == null) {
                return NotFound();
            }

            var lmToBePatched = new LandMarkForUpdateDTO() {
                Name = landMarkToUpdate.Name,
                Description = landMarkToUpdate.Description
            };

            patchDoc.ApplyTo(lmToBePatched, ModelState);

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(lmToBePatched)) {
                return BadRequest(ModelState);
            }

            landMarkToUpdate.Name = lmToBePatched.Name;
            landMarkToUpdate.Description = lmToBePatched.Description;

            return NoContent();
        }

        [HttpDelete("{landMarkID}")]
        public ActionResult DeleteLandMark(int cityID,
            int landMarkID
            ) {

            var city = DataStores.CitiesDataStore.Current.FirstOrDefault(c => c.ID == cityID);

            if (city == null) {
                return NotFound();
            }

            var landMarkToDelete = city.LandMarks.FirstOrDefault(lm => lm.ID == landMarkID);

            if (landMarkToDelete == null) {
                return NotFound();
            }

            ((List<LandMarkDTO>)city.LandMarks).Remove(landMarkToDelete);

            return NoContent();
        }
    }
}
