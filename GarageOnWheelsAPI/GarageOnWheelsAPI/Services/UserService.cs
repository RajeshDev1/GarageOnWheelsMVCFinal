using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using GarageOnWheelsAPI.Data;
using GarageOnWheelsAPI.DTOs;
using GarageOnWheelsAPI.Enums;
using GarageOnWheelsAPI.Interfaces.IServices;
using GarageOnWheelsAPI.Interfaces.Repositories;
using GarageOnWheelsAPI.Models.DatabaseModels;
using GarageOnWheelsAPI.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GarageOnWheelsAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public UserService(IUserRepository userRepository, IConfiguration configuration, ApplicationDbContext context)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _context = context;
        }


        public async Task AddUserAsync(UserDto userDto)
        {

            userDto.Password = PasswordHasher.HashPassword(userDto.Password);
            userDto.UpdatedBy = userDto.CreatedBy;
            userDto.Id = Guid.NewGuid();
            userDto.CreatedDate = DateTime.Now;
            userDto.UpdatedDate = userDto.CreatedDate;
      
            var user = UserDto.mapping(userDto);
            await _userRepository.AddUserAsync(user);
        }

   

        public async Task<UserDto> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            var userDto = UserDto.mapping(user);

            return userDto;
            
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            var userDtos = UserDto.mapping(users);
            return userDtos;
        }

        public async Task<IEnumerable<UserDto>> GetAllCustomersAsync()
        {
            var users = await _userRepository.GetAllCustomersAsync();
            var userDtos = UserDto.mapping(users);
            return userDtos;
        }

        public async Task UpdateUserAsync(UpdateUserDto userDto)
        {
            var existingUser = await _userRepository.GetUserByIdAsync(userDto.Id);
            if (existingUser == null) throw new Exception("User not found");

            existingUser.FirstName = userDto.FirstName;
            existingUser.LastName = userDto.LastName;
            existingUser.Email = userDto.Email;
            existingUser.Role = (int)userDto.Role;
            existingUser.PhoneNo = userDto.PhoneNo;
            existingUser.Address = userDto.Address;
            existingUser.UpdatedBy = userDto.UpdatedBy;
            existingUser.UpdatedDate = DateTime.Now;
            existingUser.AreaId = userDto.AreaId;
            existingUser.CityId = userDto.CityId;
            existingUser.StateId = userDto.StateId;
            existingUser.Gender = Enum.GetName(typeof(Gender), userDto.Gender);
            existingUser.CountryId = userDto.CountryId;
            existingUser.IsDelete = userDto.IsDelete;
            existingUser.IsEmailVerified = userDto.IsEmailVerified;
            

            await _userRepository.UpdateUserAsync(existingUser);
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) throw new Exception("User not found");
            user.IsDelete = true;
            await _userRepository.UpdateUserAsync(user);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            if (string.IsNullOrEmpty(role))
            {
                throw new ArgumentException("Role cannot be null or empty", nameof(role));
            }

            var users = await _userRepository.GetAllUsersAsync();
            var usersByRole = users.Where(u => u.Role.ToString().Equals(role, StringComparison.OrdinalIgnoreCase)).ToList();

            return usersByRole;
        }

        public async Task<UserDto> GetUserByEmailAsync(string email, string password)
        {
            var user = await _userRepository.getUserEmailAsync(email,password);
            if (user == null)
            {
                throw new Exception("User not Found");
            }
            var userDto = UserDto.mapping(user);
            return userDto;
        }

        public async Task<UserDto> GetEmailAsync(string email)
        {
            var user = await _userRepository.getEmailAsync(email);
            if (user == null)
            {
                throw new Exception("User not Found");
            }
            var userDto = UserDto.mapping(user);
            return userDto;
        }


        public async Task<bool> EmailExistAsync(string email)
        {
            return await _userRepository.EmailExistsAsync(email);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersByRoleAsync(string role)
        {
            Enum.TryParse(role,true,out UserRole Role);

            var users  = await _userRepository.GetAllUsersByRoleAsync(Role);
            if(users == null)
            {
                throw new Exception("User not Found");
            }
            var userDtos = UserDto.mapping(users);
            return userDtos;
        }

        public async Task<IdentityResult> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }
            if (user.PasswordHash != PasswordHasher.HashPassword(currentPassword))
            {
                return IdentityResult.Failed(new IdentityError { Description = "Old Password is not Matched." });
            }
            if (currentPassword == newPassword)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Old Password and New Password must be different." });
            }
            user.PasswordHash = PasswordHasher.HashPassword(newPassword);
            await _userRepository.UpdateUserAsync(user);
            return IdentityResult.Success;
        }

        public async Task<IEnumerable<UserDto>> GetAllGarageOwnersAsync()
        {
            var users = await _userRepository.GetAllGarageOwnersAsync();
            var userDtos = UserDto.mapping(users);
            return userDtos;
        }
    }
}
