using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GarageOnWheelsAPI.Data;
using GarageOnWheelsAPI.Interfaces.Repositories;
using GarageOnWheelsAPI.Models.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace GarageOnWheelsAPI.Repositories
{
    public class GarageRepository : IGarageRepository
    {
        private readonly ApplicationDbContext _context;

        public GarageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all garages
        public async Task<IEnumerable<Garage>> GetAllGaragesAsync()
        {
            var query =  _context.Garages
                                 .Include(g => g.User)
                                 .Include(g => g.Country)
                                 .Include(g => g.State)
                                 .Include(g => g.City)
                                 .Include(g => g.Area);

                 return await query.Where(g => !g.IsDelete).ToListAsync();

        }

        // Get a garage by ID
        public async Task<Garage?> GetGarageByIdAsync(Guid garageId)
        {
            var query = _context.Garages
                                 .Include(g => g.User)
                                 .Include(g => g.Country)
                                 .Include(g => g.State)
                                 .Include(g => g.City)
                                 .Include(g => g.Area);

           return await query.FirstOrDefaultAsync(g => g.Id == garageId && !g.IsDelete);
        }


        public async Task AddGarageAsync(Garage garage)
        {
             _context.Garages.AddAsync(garage);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateGarageAsync(Garage garage)
        {
            _context.Garages.Update(garage);
            await _context.SaveChangesAsync();
        }


        public async Task DeleteGarageAsync(Guid garageId)
        {
            var garage = await _context.Garages.FindAsync(garageId);
            if (garage != null)
            {
                garage.IsDelete = true;
                _context.Garages.Update(garage);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Garage> GetGarageByOwnerIdAsync(Guid ownerId)
        {
            {
                return await _context.Garages
                    .FirstOrDefaultAsync(g => g.UserId == ownerId && !g.IsDelete);
            }
        }

        public async Task<IEnumerable<Garage>> GetGaragesByUserIdAsync(Guid userId)
        {
            var query = _context.Garages
                                 .Include(g => g.User)
                                 .Include(g => g.Country)
                                 .Include(g => g.State)
                                 .Include(g => g.City)
                                 .Include(g => g.Area)
                                 .Where(g => g.UserId == userId && !g.IsDelete);
             return await query.ToListAsync();
        }


        // Check if garage name already exists
        public async Task<bool> GarageExistsByUserIdAsync(Guid ownerId)
        {
            return await _context.Garages.AnyAsync(g => g.UserId == ownerId && !g.IsDelete);
        }
    }
}
