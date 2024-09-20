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
       Task CreateOrderAsync(Orders order);

        //Retrieves an order by its ID.
        Task<Orders> GetOrderByIdAsync(Guid id);

        //Retrieves all orders from the database.
        Task<IEnumerable<Orders>> GetAllOrdersAsync();

        //Retrieves all orders associated with a specific user.
        Task<IEnumerable<Orders>> GetOrdersByUserIdAsync(Guid userId);

        //Retrieves all orders associated with a specific garage.
        Task<IEnumerable<Orders>> GetOrdersByGarageIdAsync(Guid garageId);

        //Updates an existing order.
        Task UpdateOrderAsync(Orders order);

        //Deletes an order by its ID.
        Task DeleteOrderAsync(Guid id);

        //get order on a specific date
        Task<IEnumerable<Orders>> GetOrdersBySpecificDateAsync(DateTime date);

        // Get orders by date range
        Task<IEnumerable<Orders>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
