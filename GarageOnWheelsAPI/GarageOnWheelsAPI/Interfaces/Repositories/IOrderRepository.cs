using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;
using GarageOnWheelsAPI.Models.DatabaseModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace GarageOnWheelsAPI.Interfaces.Repositories
{
    public interface IOrderRepository
    {

        //Adds a new order to the database.
        Task<Order> AddOrderAsync(Order order);

        //Retrieves an order by its ID.
        Task<Order> GetOrderByIdAsync(Guid id);

        //Retrieves all orders from the database.
        Task<IEnumerable<Order>> GetAllOrdersAsync();

        //Retrieves all orders associated with a specific user.
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId);

        //Retrieves all orders associated with a specific garage.
        Task<IEnumerable<Order>> GetOrdersByGarageIdAsync(Guid garageId);

        //Updates an existing order.
        Task UpdateOrderAsync(Order order);

        //Deletes an order by its ID.
        Task DeleteOrderAsync(Guid id);

        //get order on a specific date
        Task<IEnumerable<Order>> GetOrdersBySpecificDateAsync(DateTime date);

        // Get orders by date range
        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);

        Task AddOrderFilesAsync(List<OrderFiles> orderFiles);

    }
}
