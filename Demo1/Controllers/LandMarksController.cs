using Demo1.DataStores;
using Demo1.DTO;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Demo1.Controllers {
    [ApiController]
    [Route("api/cities/{cityID}/landmarks")]
    public class LandMarksController : Controller {
        [HttpGet]
        public ActionResult<IEnumerable<LandMarkDTO>> GetLandMarks(int cityID) {
            var city = DataStores.CitiesDataStore.Current.FirstOrDefault(c => c.ID == cityID);

            if (city == null) {
                return NotFound();
            }

            return Ok(city.LandMarks);
        }

        [HttpGet("{landMarkID}"/*, Name = "GetLandMark"*/)]
        public ActionResult<LandMarkDTO> GetLandMark(int cityID, int landMarkID) {
            var city = DataStores.CitiesDataStore.Current.FirstOrDefault(c => c.ID == cityID);

            if (city == null) {
                return NotFound();
            }

            var landMark = city.LandMarks.FirstOrDefault(l => l.ID == landMarkID);

            if (landMark == null) {
                return NotFound();
            }

            return Ok(landMark);
        }

        [HttpPost]
        public ActionResult<LandMarkDTO> AddLandMark(int cityID, LandMarkForCreateDTO newLandMark) {
            var city = DataStores.CitiesDataStore.Current.FirstOrDefault(c => c.ID == cityID);

            if (city == null) {
                return NotFound();
            }

            var lastID = CitiesDataStore.Current.SelectMany(c => c.LandMarks).Max(lm => lm.ID);

            var finalLandMark = new LandMarkDTO {
                ID = ++lastID,
                Name = newLandMark.Name,
                Description = newLandMark.Description
            };

            ((List<LandMarkDTO>)city.LandMarks).Add(finalLandMark);


            // If enabling Route then dont forget to reenable the route name above in the GetLandMark method
            //return CreatedAtRoute(
            //    "GetLandMark", 
            //    new { 
            //        cityID = cityID,
            //        landMarkID = finalLandMark.ID
            //    }, 
            //    finalLandMark
            //);


            return CreatedAtAction(
                nameof(GetLandMark),
                new {
                    cityID = cityID,
                    landMarkID = finalLandMark.ID
                },
                finalLandMark
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

            patchDoc.ApplyTo(lmToBePatched);

            landMarkToUpdate.Name = lmToBePatched.Name;
            landMarkToUpdate.Description = lmToBePatched.Description;

            // TODO: add validations!!!


            return NoContent();
        }
    }
}
