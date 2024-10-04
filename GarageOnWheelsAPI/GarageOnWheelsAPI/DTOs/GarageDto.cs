using GarageOnWheelsAPI.Enums;
using GarageOnWheelsAPI.Models.DatabaseModels;
using System.ComponentModel.DataAnnotations;

namespace GarageOnWheelsAPI.DTOs
{
    public class GarageDto
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } 
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string Address { get; set; } 
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public DateTime UpdatedDate { get; set; }
        [Required]
        public Guid CreatedBy { get; set; }
        [Required]
        public Guid UpdatedBy { get; set; }
        [Required]
        public bool IsDelete { get; set; } = false;
        [Required]
        public int CountryId { get; set; }
        [Required]
        public int StateId { get; set; }
        [Required]
        public int CityId { get; set; }
        [Required]
        public int AreaId { get; set; }

        public  Country ? Country { get; set; }
        public  City ? City { get; set; }
        public State ? State { get; set; }
        public  Area ? Area { get; set; }

        public string ? GarageOwnerName { get; set; }

        public string? Email { get; set; }



        public static Garage Mapping(GarageDto garageDto)
        {
            return new Garage
            {
                Id = garageDto.Id,
                Name = garageDto.Name,
                Address = garageDto.Address,
                UserId = garageDto.UserId,
                CreatedDate = garageDto.CreatedDate,
                CreatedBy = garageDto.UserId,
                UpdatedDate = garageDto.UpdatedDate,
                UpdatedBy = garageDto.UserId,
                IsDelete = garageDto.IsDelete,
                CountryId = garageDto.CountryId,
                StateId = garageDto.StateId,
                CityId = garageDto.CityId,
                AreaId = garageDto.AreaId,
               
            };
        }

        public static IEnumerable<Garage> Mapping(IEnumerable<GarageDto> garageDtos)
        {
            var garages = new List<Garage>();
            foreach (var garage in garageDtos)
            {
                garages.Add(Mapping(garage));
            }
            return garages;
        } 


        public static GarageDto Mapping(Garage garage)
        {
            return new GarageDto
            {
                Id = garage.Id,
                Name = garage.Name,
                Address = garage.Address,
                UserId = garage.UserId,
                CreatedDate = garage.CreatedDate,
                UpdatedDate = garage.UpdatedDate,
                GarageOwnerName = garage.User?.FirstName+" "+garage.User?.LastName,
                CreatedBy = garage.UserId,
                UpdatedBy = garage.UserId,
                AreaId = garage.AreaId,
                CountryId = garage.CountryId,
                StateId = garage.StateId,
                CityId = garage.CityId,
                IsDelete = garage.IsDelete,
                Country = garage.Country,
                State = garage.State,
                City = garage.City,
                Area = garage.Area,
                Email = garage.User?.Email
               
            };
        }
        public static IEnumerable<GarageDto> Mapping(IEnumerable<Garage> garages) 
        { 
            var garageDtos = new List<GarageDto>();
            foreach (var garage in garages)
            {
                garageDtos.Add(Mapping(garage));
            }
            return garageDtos;
        }


     


    }
}
