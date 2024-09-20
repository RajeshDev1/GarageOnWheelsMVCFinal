using GarageOnWheelsAPI.Interfaces.IServices;
using GarageOnWheelsAPI.Interfaces.Repositories;
using GarageOnWheelsAPI.Models.DatabaseModels;

namespace GarageOnWheelsAPI.Services
{
    public class LocationService:ILocationService
    {
        private readonly ILocationRepository _locationRepository;
        public LocationService(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }
        public async Task<IEnumerable<Country>> GetAllCountries()
        {
            return await _locationRepository.GetAllCountries();
        }
        public async Task<IEnumerable<State>> GetAllStates(int CountryId)
        {
            return await _locationRepository.GetAllStates(CountryId);
        }
        public async Task<IEnumerable<City>> GetAllCities(int StateId)
        {
            return await _locationRepository.GetAllCities(StateId);
        }

        public async Task<IEnumerable<Area>> GetAllAreas(int CityId)
        {
            return await _locationRepository.GetAllAreas(CityId);
        }
    }
}



