using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using GarageOnWheelsAPI.DTOs;
using GarageOnWheelsAPI.Enums;
using GarageOnWheelsAPI.Models.DatabaseModels;

namespace GarageOnWheelsAPI.Interfaces.Repositories
{
    public interface IUserRepository
    {

     
        Task<IEnumerable<User>> GetAllCustomersAsync();
        Task<IEnumerable<User>> GetAllGarageOwnersAsync();
        Task<User> GetUserByIdAsync(Guid id);
        Task AddUserAsync(User user);
        Task  UpdateUserAsync(User user);
        Task<bool> EmailExistsAsync(string email);
        Task<User> getUserEmailAsync(string email,string password);
        Task<User> getEmailAsync(string email);
        Task<IEnumerable<User>> GetAllUsersByRoleAsync(UserRole role);
        Task<IEnumerable<User>> GetAllUsersAsync();

    }
}
