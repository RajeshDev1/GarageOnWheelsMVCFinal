using Microsoft.AspNetCore.Mvc;
using GarageOnWheelsMVC.Service;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GarageOnWheelsMVC.Controllers
{
    public class LocationController : Controller
    {
        private readonly LocationService _locationService;

        public LocationController(LocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet]
        public async Task<JsonResult> GetCountries()
        {
            var countries = await _locationService.GetCountriesAsync();
            return new JsonResult(countries);
        }

        [HttpGet]
        public async Task<JsonResult> GetStates(int Id)
        {
            var states = await _locationService.GetStatesAsync(Id);
            return new JsonResult(states);
        }

        [HttpGet]
        public async Task<JsonResult> GetCities(int Id)
        {
            var cities = await _locationService.GetCitiesAsync(Id);
            return new JsonResult(cities);
        }

        [HttpGet]
        public async Task<JsonResult> GetAreas(int Id)
        {
            var areas = await _locationService.GetAreasAsync(Id);
            return new JsonResult(areas);
        }
    }
}
