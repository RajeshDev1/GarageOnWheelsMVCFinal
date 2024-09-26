using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GarageOnWheelsAPI.DTOs;
using GarageOnWheelsAPI.Interfaces.IServices;
using GarageOnWheelsAPI.Interfaces.Repositories;
using GarageOnWheelsAPI.Models.DatabaseModels;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GarageOnWheelsAPI.Services
{ 
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        // Get all orders
        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var order = await _orderRepository.GetAllOrdersAsync();
            var orderDto = OrderDto.Mapping(order);
            return orderDto;
        }

        // Get an order by ID
        public async Task<OrderDto> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            var orderDto = OrderDto.Mapping(order);
            return orderDto;
        }

        // Create a new order
        public async Task CreateOrderAsync(OrderDto orderDto)
        {
            var order = OrderDto.Mapping(orderDto);
            await _orderRepository.CreateOrderAsync(order);
        }

        // Update an existing order
        public async Task UpdateOrderAsync(OrderDto order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            // Check if the order exists before attempting to update
            var existingOrder = await _orderRepository.GetOrderByIdAsync(order.Id);
            if (existingOrder == null)
            {
                throw new InvalidOperationException("Order not found");
            }

            // Update the order details
            existingOrder.UserId = order.UserId;
            existingOrder.TotalAmount = order.TotalAmount;
            existingOrder.Status = (int)order.Status;
            existingOrder.UpdatedBy = order.UpdatedBy;
            existingOrder.UpdatedDate = DateTime.Now;
            existingOrder.ServiceDetails = order.ServiceDetails;
            existingOrder.IsDelete = order.IsDelete;

            await _orderRepository.UpdateOrderAsync(existingOrder);
        }

        // Delete an order (soft delete)
        public async Task DeleteOrderAsync(Guid orderId)
        {
            await _orderRepository.DeleteOrderAsync(orderId);
        }

        // Get orders by user ID
        public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(Guid userId)
        {
            var order = await _orderRepository.GetOrdersByUserIdAsync(userId);
            var orderDto = OrderDto.Mapping(order);
            return orderDto;
        }

        // Get orders by garage ID
        public async Task<IEnumerable<OrderDto>> GetOrdersByGarageIdAsync(Guid garageId)
        {
            var order = await _orderRepository.GetOrdersByGarageIdAsync(garageId);
            var orderDtos = OrderDto.Mapping(order);
            return orderDtos;
        }

        // Get orders on a specific date
        public async Task<IEnumerable<OrderDto>> GetOrdersBySpecificDateAsync(DateTime date)
        {
            var order = await _orderRepository.GetOrdersBySpecificDateAsync(date);
            var orderDto = OrderDto.Mapping(order);
            return orderDto;
        }

        // Get orders by date range
        public async Task<IEnumerable<OrderDto>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var order = await _orderRepository.GetOrdersByDateRangeAsync(startDate, endDate);
            var orders = OrderDto.Mapping(order);
            return orders;
        }
    }
}
