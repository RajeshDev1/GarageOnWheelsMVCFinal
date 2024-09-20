using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GarageOnWheelsAPI.DTOs;
using GarageOnWheelsAPI.Enums;
using GarageOnWheelsAPI.Models.DatabaseModels;

namespace GarageOnWheelsAPI.Interfaces.IServices
{
    public interface IOrderService
    {
        // Get all orders
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();

        // Get an order by ID
        Task<OrderDto> GetOrderByIdAsync(Guid orderId);

        // Create a new order
        Task CreateOrderAsync(OrderDto orderDto);

        // Update an existing order
        Task UpdateOrderAsync(OrderDto orderDto);

        // Delete an order (soft delete)
        Task DeleteOrderAsync(Guid orderId);

        // Get orders by user ID
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(Guid userId);

        // Get orders by garage ID
        Task<IEnumerable<OrderDto>> GetOrdersByGarageIdAsync(Guid garageId);

        //get order on a specific date
        Task<IEnumerable<OrderDto>> GetOrdersBySpecificDateAsync(DateTime date);

        // Get orders by date range
        Task<IEnumerable<OrderDto>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
