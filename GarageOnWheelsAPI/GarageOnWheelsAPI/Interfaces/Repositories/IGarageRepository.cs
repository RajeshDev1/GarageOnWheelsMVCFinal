using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GarageOnWheelsAPI.Models.DatabaseModels;

namespace GarageOnWheelsAPI.Interfaces.Repositories
{
    public interface IGarageRepository
    {
        // Get all garages
        Task<IEnumerable<Garage>> GetAllGaragesAsync();

        // Get a garage by ID
        Task<Garage?> GetGarageByIdAsync(Guid garageId);

        // Add a new garage
        Task AddGarageAsync(Garage garage);

        // Update an existing garage
        Task UpdateGarageAsync(Garage garage);

        // Delete a garage (soft delete)
        Task DeleteGarageAsync(Guid garageId);

        // Get garages by user ID
        Task<IEnumerable<Garage>> GetGaragesByUserIdAsync(Guid userId);

        // Check if garage name already exists
        Task<bool> GarageExistsByUserIdAsync(Guid ownerId);

        Task<Garage> GetGarageByOwnerIdAsync(Guid ownerId);

        Task<IEnumerable<Garage>> GetGaragesByCityIdAsync(int cityId);
    }

}
