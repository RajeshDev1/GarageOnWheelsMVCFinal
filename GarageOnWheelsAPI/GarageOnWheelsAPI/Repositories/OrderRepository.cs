using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GarageOnWheelsAPI.Data;
using GarageOnWheelsAPI.Enums;
using GarageOnWheelsAPI.Interfaces.Repositories;
using GarageOnWheelsAPI.Models.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace GarageOnWheelsAPI.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Adds a new order to the database.
        public async Task CreateOrderAsync(Orders order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

             _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        // Retrieves an order by its ID.
        public async Task<Orders> GetOrderByIdAsync(Guid id)
        {
            return await _context.Orders.FindAsync(id);
        }

        // Retrieves all orders from the database.
        public async Task<IEnumerable<Orders>> GetAllOrdersAsync()
        {
            var query = _context.Orders
                 .Include(o => o.Garage)
                 .Include(a => a.User)
                 .Where(o => !o.IsDelete);
                return await query.ToListAsync();
        }


        // Retrieves all orders associated with a specific user, sorted by the most recent order.
        public async Task<IEnumerable<Orders>> GetOrdersByUserIdAsync(Guid userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.UpdatedDate).ToListAsync();
        }



        // Retrieves all orders associated with a specific garage.
        public async Task<IEnumerable<Orders>> GetOrdersByGarageIdAsync(Guid garageId)
        {
            return await _context.Orders.Where(o => o.GarageId == garageId && !o.IsDelete).OrderByDescending(o => o.CreatedDate).ToListAsync();
        }

        // Updates an existing order.

        public async Task UpdateOrderAsync(Orders order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

               _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        // Deletes an order by its ID (soft delete by setting IsDelete to true).
        public async Task DeleteOrderAsync(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                order.IsDelete = true;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
            }
        }   

        // Retrieves all orders placed on a specific date.
        public async Task<IEnumerable<Orders>> GetOrdersBySpecificDateAsync(DateTime date)
        {
            return await _context.Orders
                .Where(o => o.OrderDate.Date == date.Date) 
                .ToListAsync();
        }

        // Retrieves all orders within a specific date range.
        public async Task<IEnumerable<Orders>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Orders
                .Where(o => o.OrderDate.Date >= startDate.Date && o.OrderDate.Date <= endDate.Date)
                .ToListAsync();
        }
    }
}
