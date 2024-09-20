using GarageOnWheelsAPI.Models.DatabaseModels;

namespace GarageOnWheelsAPI.Interfaces.Repositories
{
    public interface ILocationRepository
    {
        Task<IEnumerable<Country>> GetAllCountries();
        Task<IEnumerable<State>> GetAllStates(int CountryId);
        Task<IEnumerable<City>> GetAllCities(int StateId);
        Task<IEnumerable<Area>> GetAllAreas(int CityId);
    }
}
