﻿using GarageOnWheelsAPI.Enums;
using GarageOnWheelsAPI.Models.DatabaseModels;
using System.ComponentModel.DataAnnotations;

namespace GarageOnWheelsAPI.DTOs
{
    public class UpdateUserDto
    {

        public Guid Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First Name must be between 2 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s\(\)]+$", ErrorMessage = "First Name must contain only alphabets.")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last Name must be between 2 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s\(\)]+$", ErrorMessage = "Last Name must contain only alphabets.")]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" + @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid Email Format")]
        public string Email { get; set; }

        [Required]
        public UserRole Role { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNo { get; set; }
        [Required(ErrorMessage = "Gender is required")]
        public Gender Gender { get; set; } = Gender.Male;
        [Required]
        public string Address { get; set; }
        public bool IsEmailVerified { get; set; } = false;

        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        [Required]
        public int CountryId { get; set; }
        [Required]
        public int StateId { get; set; }
        [Required]
        public int CityId { get; set; }
        [Required]
        public int AreaId { get; set; }
        [Required]
        public bool IsDelete { get; set; } = false;
        public string? ProfileImage { get; set; }

        public static UserDto mapping(UpdateUserDto userDto)
        {
            return new UserDto()
            {
                Id = userDto.Id,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email, 
                Role = userDto.Role,
                Address = userDto.Address,
                PhoneNo = userDto.PhoneNo,
                IsEmailVerified = userDto.IsEmailVerified,               
                UpdatedDate = userDto.UpdatedDate,
                AreaId = userDto.AreaId,
                CountryId = userDto.CountryId,
                StateId = userDto.StateId,
                CityId = userDto.CityId,
                Gender = userDto.Gender,
                IsDelete = userDto.IsDelete,
                UpdatedBy = userDto.UpdatedBy,
                ProfileImage = userDto.ProfileImage,

            };

        }

        public static UpdateUserDto mapping(UserDto user)
        {
            return new UpdateUserDto()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,      
                Role = user.Role,
                Address = user.Address,
                PhoneNo = user.PhoneNo,
                Gender =  user.Gender,
                IsEmailVerified = user.IsEmailVerified,
                UpdatedDate = user.UpdatedDate,
                AreaId = user.AreaId,
                CountryId = user.CountryId,
                StateId = user.StateId,
                CityId = user.CityId,
                IsDelete = user.IsDelete,            
                UpdatedBy = user.UpdatedBy,
                ProfileImage = user.ProfileImage
            };

        }

        public static IEnumerable<UpdateUserDto> mapping(IEnumerable<UserDto> users)
        {
            var userDtos = new List<UpdateUserDto>();
            foreach (var user in users)
            {
                userDtos.Add(mapping(user));
            }
            return userDtos;
        }

        public static IEnumerable<UserDto> mapping(IEnumerable<UpdateUserDto> userDto)
        {
            var userList = new List<UserDto>();
            foreach (var user in userDto)
            {
                userList.Add(mapping(user));
            }
            return userList;
        }

    }
}
