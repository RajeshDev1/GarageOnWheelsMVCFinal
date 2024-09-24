using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GarageOnWheelsAPI.Data;
using GarageOnWheelsAPI.Enums;
using GarageOnWheelsAPI.Interfaces.Repositories;
using GarageOnWheelsAPI.Models.DatabaseModels;
using Microsoft.EntityFrameworkCore;
using GarageOnWheelsAPI.Interfaces;
using GarageOnWheelsAPI.Services;
using GarageOnWheelsAPI.Interfaces.IServices;
using GarageOnWheelsAPI.DTOs;
using System.Data;


namespace GarageOnWheelsAPI.Repositories
{
    public class UserRepository : IUserRepository
    {

        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }
      
        public async Task AddUserAsync(User user)  
        {

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateUserAsync(User user)
        {
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Occured : {ex.Message}");
                throw;
            }
        }

        // Get all garageowner,Customer
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Where(u => (int)u.Role != (int)UserRole.SuperAdmin && u.IsDelete == false).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllCustomersAsync()
        {
            return await _context.Users
                .Where(u => (int)u.Role == (int)UserRole.Customer && u.IsDelete == false).ToListAsync();
        }



        public async Task<IEnumerable<User>> GetAllGarageOwnersAsync()
        {
            return await _context.Users
                .Where(u => (int)u.Role == (int)UserRole.GarageOwner && u.IsDelete == false).ToListAsync();
        }



        public async Task<bool> EmailExistsAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email ); 
            return user != null;
        }

        public async Task<User> getEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            
        }

        public async Task<User> getUserEmailAsync(string email,string Password)
        {
            var query = _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsEmailVerified && u.PasswordHash == Password && !u.IsDelete);
            return await query;
            
        }

        public async Task<IEnumerable<User>> GetAllUsersByRoleAsync(UserRole role)
        {
            var query = _context.Users.Where(u => u.IsDelete == false && u.Role == (int)role && !u.IsEmailVerified);
            return await query.ToListAsync();
        }
    }
}
