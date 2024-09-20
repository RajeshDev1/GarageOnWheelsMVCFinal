using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GarageOnWheelsAPI.DTOs;
using GarageOnWheelsAPI.Interfaces.IServices;
using GarageOnWheelsAPI.Interfaces.Repositories;
using GarageOnWheelsAPI.Models.DatabaseModels;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GarageOnWheelsAPI.Services
{
    public class GarageService : IGarageService
    {
        private readonly IGarageRepository _garageRepository;

        public GarageService(IGarageRepository garageRepository)
        {
            _garageRepository = garageRepository;
        }

        // Add a new garage
        public async Task CreateGarageAsync(GarageDto garage)
        {
            garage.Id = Guid.NewGuid();
            garage.CreatedDate = DateTime.Now;
            garage.CreatedBy = garage.UserId;
            garage.UpdatedBy = garage.CreatedBy;
            garage.UpdatedDate = garage.CreatedDate;

            var garageDto = GarageDto.Mapping(garage);

            await _garageRepository.AddGarageAsync(garageDto);
        }

        // Get a garage by ID
        public async Task<GarageDto> GetGarageByGarageIdAsync(Guid garageId)
        {
            var garage  = await _garageRepository.GetGarageByIdAsync(garageId);
            if (garage == null) 
            {
                throw new Exception("Garage not Found");
            }
            var garageDto = GarageDto.Mapping(garage);
            return garageDto;
        }

        // Get all garages
        public async Task<IEnumerable<GarageDto>> GetAllGaragesAsync()
        {
            var garage = await _garageRepository.GetAllGaragesAsync();
            if (garage == null)
            {
                throw new Exception("Garage not Found");
            }
            var garagedto = GarageDto.Mapping(garage);

            return garagedto;
        }

        // Update an existing garage
        public async Task UpdateGarageAsync(GarageDto garagedto)
        {
            var exist = await _garageRepository.GetGarageByIdAsync(garagedto.Id);
            if (exist == null) 
            {
                throw new Exception("Garage not Found !!");
            }
            exist.Id = garagedto.Id;
            exist.UserId = garagedto.UserId;
            exist.UpdatedDate = DateTime.Now;
            exist.Address = garagedto.Address;
            exist.AreaId = garagedto.AreaId;
            exist.Address = garagedto.Address;  
            exist.CityId = garagedto.CityId;
            exist.StateId = garagedto.StateId;
            exist.CountryId = garagedto.CountryId;
            exist.IsDelete = garagedto.IsDelete;
            exist.UpdatedBy = garagedto.UserId;


            await _garageRepository.UpdateGarageAsync(exist);
            
        }

        // Delete a garage (soft delete)
        public async Task DeleteGarageAsync(Guid garageId)
        {
            await _garageRepository.DeleteGarageAsync(garageId);
        }

        // Check if garage name already exists
        public async Task<bool> GarageExistsByUserIdAsync(Guid userId)
        {
            return await _garageRepository.GarageExistsByUserIdAsync(userId);
        }

        // Get garages by user ID
        public async Task<IEnumerable<GarageDto>> GetGaragesByUserIdAsync(Guid userId)
        {
            var garage = await _garageRepository.GetGaragesByUserIdAsync(userId);
            if (garage == null)
            {
                throw new Exception("Garage not Found");
            }
            var garageDto = GarageDto.Mapping(garage);

            return garageDto;
        }

        public async Task<GarageDto> GetGarageByOwnerIdAsync(Guid ownerId)
        {
            var garage = await _garageRepository.GetGarageByOwnerIdAsync(ownerId);
            if(garage == null)
            {
                throw new Exception("Garage not Found");
            }
            var garageDto = GarageDto.Mapping(garage);
            return garageDto;
        }

    }
}
