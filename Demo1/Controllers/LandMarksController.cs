using Microsoft.AspNetCore.Mvc;

namespace Demo1.Controllers {
    [ApiController]
    [Route("api/cities/{cityID}/landmarks")]
    public class LandMarksController : Controller {
        [HttpGet]
        public ActionResult GetLandMarks(int cityID) {
            var city = DataStores.CitiesDataStore.Current.FirstOrDefault(c => c.ID == cityID);

            if (city == null) {
                return NotFound();
            }

            return Ok(city.LandMarks);
        }
    }
}
