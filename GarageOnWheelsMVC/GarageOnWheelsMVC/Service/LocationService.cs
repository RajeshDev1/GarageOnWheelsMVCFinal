using GarageOnWheelsMVC.Models;
using System.Diagnostics.Metrics;

namespace GarageOnWheelsMVC.Service
{
    public class LocationService
    {
        private readonly HttpClient _httpClient;

        public LocationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Country>> GetCountriesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<Country>>("api/Location/Countries");
            return response ?? new List<Country>();
        }

        public async Task<List<State>> GetStatesAsync(int countryId)
        {
            var response = await _httpClient.GetFromJsonAsync<List<State>>($"api/Location/States/{countryId}");
            return response ?? new List<State>();
        }

        public async Task<List<City>> GetCitiesAsync(int stateId)
        {
            var response = await _httpClient.GetFromJsonAsync<List<City>>($"api/Location/Cities/{stateId}");
            return response ?? new List<City>();
        }

        public async Task<List<Area>> GetAreasAsync(int cityId)
        {
            var response = await _httpClient.GetFromJsonAsync<List<Area>>($"api/Location/Areas/{cityId}");
            return response ?? new List<Area>();
        }
    }

}
