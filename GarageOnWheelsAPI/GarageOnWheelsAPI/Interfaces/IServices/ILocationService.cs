using GarageOnWheelsAPI.Models.DatabaseModels;

namespace GarageOnWheelsAPI.Interfaces.IServices
{
    public interface ILocationService
    {
        Task<IEnumerable<Country>> GetAllCountries();
        Task<IEnumerable<State>> GetAllStates(int CountryId);
        Task<IEnumerable<City>> GetAllCities(int StateId);
        Task<IEnumerable<Area>> GetAllAreas(int CityId);
    }
}
