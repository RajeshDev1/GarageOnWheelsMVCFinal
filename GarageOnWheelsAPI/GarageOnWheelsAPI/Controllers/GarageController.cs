using GarageOnWheelsAPI.DTOs;
using GarageOnWheelsAPI.Enums;
using GarageOnWheelsAPI.Interfaces.IServices;
using GarageOnWheelsAPI.Models.DatabaseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GarageOnWheelsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GarageController : ControllerBase
    {
        private readonly IGarageService _garageService;
        private readonly IOrderService _orderService;
        private readonly ILogger<GarageController> _logger;

        public GarageController(
            IGarageService garageService,
            IOrderService orderService,
            ILogger<GarageController> logger)
        {
            _garageService = garageService;
            _orderService = orderService;
            _logger = logger;
        }



        [HttpGet("ByCity/{cityId}")]
        public async Task<IActionResult> GetGaragesByCityId(int cityId)
        {
            try
            {
                var garages = await _garageService.GetGaragesByCityIdAsync(cityId);
                return Ok(garages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving garages.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpGet("all")]
        public async Task<IActionResult> GetAllGarages()
        {
            try
            {
                var garages = await _garageService.GetAllGaragesAsync();
                return Ok(garages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all garages.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpGet("{id:guid}")]
        [Authorize(Roles = "SuperAdmin,GarageOwner")]
        public async Task<IActionResult> GetGarageById(Guid id)
        {
            try
            {
                var garage = await _garageService.GetGarageByGarageIdAsync(id);
                if (garage == null)
                {
                    return NotFound();
                }

                return Ok(garage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the garage with ID {GarageId}.", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("by-userid/{id:guid}")]
        public async Task<IActionResult> GetGarageByUserIdAsync(Guid id)
        {
            try
            {
                var garage = await _garageService.GetGaragesByUserIdAsync(id);
                if (garage == null)
                {
                    return NotFound();
                }

                return Ok(garage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the garage with ID {userId}.", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpGet("by-specificUserId/{id:guid}")]
        public async Task<IActionResult> GetGaragesBySpecificUserIdAsync(Guid id)
        {
            try
            {
                var garage = await _garageService.GetGarageByOwnerIdAsync(id);
                if (garage == null)
                {
                    return NotFound();
                }

                return Ok(garage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the garage with ID {userId}.", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }





        [HttpPost("create")]
        [Authorize(Roles = "SuperAdmin,GarageOwner")]
        public async Task<IActionResult> CreateGarage([FromBody] GarageDto garage)
        {


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {

                await _garageService.CreateGarageAsync(garage);

                return CreatedAtAction(nameof(GetGarageById), new { id = garage.Id }, garage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new garage.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpPut("update/{id:guid}")]
        [Authorize(Roles = "SuperAdmin,GarageOwner")]
        public async Task<IActionResult> UpdateGarage(Guid id, [FromBody] GarageDto garage)
        {
            if (id != garage.Id)
            {
                return BadRequest("Garage ID mismatch");
            }

            try
            {
                await _garageService.UpdateGarageAsync(garage);


                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the garage with ID {GarageId}.", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpGet("getbyownerid")]
        public async Task<IActionResult> GetGarageByOwnerId(Guid ownerId)
        {
            try
            {
                // Fetch the garage by owner ID using the service
                var garage = await _garageService.GarageExistsByUserIdAsync(ownerId);
                return Ok(garage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the garage.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }



        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("delete-garage/{garageId}")]
        public async Task<IActionResult> DeleteGarage(Guid Id)
        {
            try
            {
                // Check if there are any pending orders for the garage
                var orders = await _orderService.GetOrdersByGarageIdAsync(Id);
                var pendingOrders = orders.Where(o => o.Status == OrderStatus.Pending).ToList();

                if (pendingOrders.Count > 0)
                {
                    return BadRequest("This garage has pending orders and cannot be deleted.");
                }
                 await _garageService.DeleteGarageAsync(Id);            
                    return NoContent();
                

                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the garage.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting garage ID: {Id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
    }
}
