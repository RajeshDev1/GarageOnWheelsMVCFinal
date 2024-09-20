using GarageOnWheelsAPI.Interfaces.IServices;
using GarageOnWheelsAPI.Models.DatabaseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GarageOnWheelsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;
        private readonly ILogger<LocationController> _logger;
        public LocationController(ILocationService locationService, ILogger<LocationController> logger)
        {
            _locationService = locationService;
            _logger = logger;
        }

        [HttpGet("Countries")]
        public async Task<IActionResult> GetCountries()
        {
            try
            {

                var countries = await _locationService.GetAllCountries();
                return Ok(countries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the Countries.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("States/{countryId}")]
        public async Task<IActionResult> GetStates(int countryId)
        {
            try
            {

                var states = await _locationService.GetAllStates(countryId);
                return Ok(states);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the states.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("Cities/{stateId}")]
        public async Task<IActionResult> Getcities(int stateId)
        {
            try
            {

                var cities = await _locationService.GetAllCities(stateId);
                return Ok(cities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the cities.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("Areas/{cityId}")]
        public async Task<IActionResult> GetAreas(int cityId)
        {
            try
            {

                var area = await _locationService.GetAllAreas(cityId);
                return Ok(area);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the areas.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
    }
}
