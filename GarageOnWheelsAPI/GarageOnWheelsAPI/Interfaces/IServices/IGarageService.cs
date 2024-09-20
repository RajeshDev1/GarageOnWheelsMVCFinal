using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GarageOnWheelsAPI.DTOs;
using GarageOnWheelsAPI.Models.DatabaseModels;

namespace GarageOnWheelsAPI.Interfaces.IServices
{
    public interface IGarageService
    {
        // Add a new garage
        Task CreateGarageAsync(GarageDto garageDto);

        // Get a garage by ID
        Task<GarageDto> GetGarageByGarageIdAsync(Guid garageId);

        // Get all garages
        Task<IEnumerable<GarageDto>> GetAllGaragesAsync();

        // Update an existing garageA
        Task UpdateGarageAsync(GarageDto garageDto);

        Task<GarageDto> GetGarageByOwnerIdAsync(Guid ownerId);

        // Delete a garage (soft delete)
        Task DeleteGarageAsync(Guid garageId);

        // Check if garage name already exists
        Task<bool> GarageExistsByUserIdAsync(Guid userId);

        // Get garages by user ID
        Task<IEnumerable<GarageDto>> GetGaragesByUserIdAsync(Guid userId);


    }
}
