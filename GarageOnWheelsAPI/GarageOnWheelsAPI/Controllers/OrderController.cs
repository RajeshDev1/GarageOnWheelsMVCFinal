using GarageOnWheelsAPI.Data;
using GarageOnWheelsAPI.DTOs;
using GarageOnWheelsAPI.Interfaces.IServices;
using GarageOnWheelsAPI.Models.DatabaseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GarageOnWheelsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IOrderService _orderService;
        private readonly ILogger<UserController> _logger;

        public OrderController(
            IOrderService orderService,      
            ILogger<UserController> logger,
            ApplicationDbContext context
            )
        {
            _orderService = orderService;        
            _logger = logger;
            _context = context;
        }


        [HttpGet("all")]
        
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all orders.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }



        [HttpGet("GetOrderById/{id}")]
        public async Task<IActionResult> GetOrderByIdAsync(Guid id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    return NotFound();
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the order with ID {OrderId}.", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }   
        }


        // Get all orders by a specific garageId

        [HttpGet("GetOrdersByGarage/{garageId}")]
        [Authorize(Roles = "GarageOwner")]
        public async Task<IActionResult> GetOrdersByGarage(Guid garageId)
        {
            try
            {               
                var orders = await _orderService.GetOrdersByGarageIdAsync(garageId);           
                if (orders == null || !orders.Any())
                {
                    return NotFound(new { Message = "No orders found for the specified garage." });
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error fetching orders for Garage ID: {GarageId}", garageId);

                return StatusCode(500, new { Message = "An error occurred while fetching orders." });
            }
        }
        [HttpPost("CreateOrder")]
        [Authorize(Roles = "GarageOwner,Customer")]       
        public async Task<IActionResult> CreateOrder(OrderDto orderDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdOrder = await _orderService.CreateOrderAsync(orderDto);

            return Ok(new { message = "Order created successfully", orderId = createdOrder.Id });
        }



        [HttpPut("UpdateOrder/{id}")]
        [Authorize(Roles = "GarageOwner")]
        public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] OrderDto order)
        {
            if (id != order.Id)
            {
                return BadRequest("Order ID mismatch");
            }

            try
            {
                await _orderService.UpdateOrderAsync(order);
                return NoContent(); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the order with ID {OrderId}.", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpDelete("DeleteOrder/{id}")]
        [Authorize(Roles = "GarageOwner")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            try
            {       
                await _orderService.DeleteOrderAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the order with ID {OrderId}.", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("GetOrderHistory/{userId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetOrderHistory(Guid userId)
        {
            try
            {
                var orders = await _orderService.GetOrdersByUserIdAsync(userId);

                if (orders == null || !orders.Any())
                {
                    return NoContent();
                }

                return new JsonResult(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving order history.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpGet("GetOrderImages/{orderId}")]
        public async Task<IActionResult> GetOrderImages(Guid orderId)
        {
            var orderFiles = await _context.OrderFiles
                .Where(of => of.OrderId == orderId)
                .Select(of => new
                {
                    of.FileName,
                    of.UploadDate
                })
                .ToListAsync();

            if (orderFiles == null || !orderFiles.Any())
            {
                return NotFound("No images found for this order.");
            }

            return Ok(orderFiles);
        }


        [HttpGet("by-garageid/{id:guid}")]
        public async Task<IActionResult> GetOrderByGarageId(Guid id)
        {
            try
            {
                var orders = await _orderService.GetOrderByGarageIdAsync(id);
                return Ok(orders);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving Orders for GarageId : {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");

            }
        }
    }
}
