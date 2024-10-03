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

        public async Task<Order> AddOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }


        // Retrieves an order by its ID.
        public async Task<Order> GetOrderByIdAsync(Guid id)
        {
            return await _context.Orders.FindAsync(id);
        }

        // Retrieves all orders from the database.
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            var query = _context.Orders
                 .Include(o => o.Garage)
                 .Include(a => a.User)
                 .Where(o => !o.IsDelete);
                return await query.ToListAsync();
        }


        // Retrieves all orders associated with a specific user, sorted by the most recent order.
        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                 .Where(o => !o.IsDelete)
                .OrderByDescending(o => o.UpdatedDate).ToListAsync();
        }



        // Retrieves all orders associated with a specific garage.
        public async Task<IEnumerable<Order>> GetOrdersByGarageIdAsync(Guid garageId)
        {
            return await _context.Orders.Where(o => o.GarageId == garageId && !o.IsDelete).OrderByDescending(o => o.CreatedDate).ToListAsync();
        }

        // Updates an existing order.

        public async Task UpdateOrderAsync(Order order)
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
        public async Task<IEnumerable<Order>> GetOrdersBySpecificDateAsync(DateTime date)
        {
            return await _context.Orders
                .Where(o => o.OrderDate.Date == date.Date) 
                .ToListAsync();
        }

        // Retrieves all orders within a specific date range.
        public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Orders
                .Where(o => o.OrderDate.Date >= startDate.Date && o.OrderDate.Date <= endDate.Date)
                .ToListAsync();
        }
        // Add Images 
        public async Task AddOrderFilesAsync(List<OrderFiles> orderFiles)
        {
            await _context.OrderFiles.AddRangeAsync(orderFiles);
            await _context.SaveChangesAsync();
        }

        public async Task<List<OrderFiles>> GetFilesByOrderIdAsync(Guid orderId)
        {
            return await _context.OrderFiles
                                 .Where(of => of.OrderId == orderId)
                                 .ToListAsync();
        }


    }
}
