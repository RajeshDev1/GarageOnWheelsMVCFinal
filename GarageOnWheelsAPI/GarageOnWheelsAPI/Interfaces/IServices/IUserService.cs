using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GarageOnWheelsAPI.DTOs;
using GarageOnWheelsAPI.Enums;
using GarageOnWheelsAPI.Models.DatabaseModels;
using Microsoft.AspNetCore.Identity;

namespace GarageOnWheelsAPI.Interfaces.IServices
{
    public interface IUserService
    {

        Task<UserDto> GetUserByIdAsync(Guid id);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();

        Task<IEnumerable<UserDto>> GetAllCustomersAsync();

        Task<IEnumerable<UserDto>> GetAllGarageOwnersAsync();

        Task<IEnumerable<UserDto>> GetAllUsersByRoleAsync(string role);
        Task AddUserAsync(UserDto userDto);
        Task UpdateUserAsync(UpdateUserDto userDto);
        Task DeleteUserAsync(Guid id);
        Task<UserDto> GetUserByEmailAsync(string email,string password);

        Task<UserDto> GetEmailAsync(string email);
        Task <bool> EmailExistAsync(string email);     
        Task<IdentityResult> ChangePasswordAsync(Guid userId,string CurrentPassword,string NewPassword);
    }
}
