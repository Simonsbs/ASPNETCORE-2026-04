using Demo1.DataStores;
using Demo1.DTO;
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
    }
}
