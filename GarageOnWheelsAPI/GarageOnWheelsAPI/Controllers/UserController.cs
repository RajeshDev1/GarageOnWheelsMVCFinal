using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GarageOnWheelsAPI.Models.DatabaseModels;
using GarageOnWheelsAPI.Interfaces.IServices;
using GarageOnWheelsAPI.Enums;
using System.Security.Claims;
using GarageOnWheelsAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using GarageOnWheelsAPI.Data;

namespace GarageOnWheelsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRevenueService _revenueService;
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _context;

        public UserController(
            IUserService userService,
            IRevenueService revenueService,
            ILogger<UserController> logger,
            ApplicationDbContext context
            )
        {
            _userService = userService;
            _revenueService = revenueService;
            _logger = logger;
            _context = context;
        }

        [HttpGet("all")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to retrieve all users.");

                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

   
        [HttpGet("allCustomer")]
        [Authorize(Roles = "GarageOwner")]
        public async Task<IActionResult> GetAllCustomer([FromQuery] Guid garageOwnerId)
        {
           
            var customers = await _userService.GetAllCustomersAsync(garageOwnerId);

            if (customers == null || !customers.Any())
            {
                return NotFound("No customers found.");
            }

            return Ok(customers);
        }

        [HttpGet("allgarageowner")]
        public async Task<IActionResult> GetAllGarageOwners()
        {
            try
            {
                var users = await _userService.GetAllGarageOwnersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to retrieve all Customers.");

                return StatusCode(500, "An error occurred while processing your request.");
            }
        }



        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to retrieve the user with ID {UserId}.", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("create")]
        [Authorize(Roles = "SuperAdmin,GarageOwner")]
        public async Task<IActionResult> Create([FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existEmail = await _userService.EmailExistAsync(userDto.Email);
            
            if(existEmail != false)
            {
                return BadRequest();
            }
            try
            {
                userDto.Id = Guid.NewGuid();
                userDto.CreatedDate = DateTime.Now;
                userDto.UpdatedDate = userDto.CreatedDate;
                userDto.UpdatedBy = userDto.CreatedBy;
                userDto.CreatedBy = userDto.CreatedBy;

                await _userService.AddUserAsync(userDto);

                return CreatedAtAction(nameof(GetUserById), new { id = userDto.Id }, userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new user with ID {UserId}.", userDto.Id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut("update/{id:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
                
            if (id != userDto.Id)
            {
                return BadRequest("UserId MisMatch");
            }

            try
            {
                await _userService.UpdateUserAsync(userDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the user with ID {UserId}.", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("delete/{id:guid}")]
        [Authorize(Roles ="SuperAdmin,GarageOwner")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                 await _userService.DeleteUserAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the user with ID {UserId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpGet("Search")]
        public async Task<IActionResult> SearchUser([FromQuery] string email = null)
        {
            try
            {
                bool user = false;
                if (!string.IsNullOrWhiteSpace(email))
                {
                    user = await _userService.EmailExistAsync(email);
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Error Occur During Search the User.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }


        [HttpGet("By-Role")]
        public async Task<IActionResult> GetUsersByRole(string role)
        {
            try
            {
                var users = await _userService.GetAllUsersByRoleAsync(role);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting users by role.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPost("change-password/{id:guid}")]
        public async Task<IActionResult> ChangePassword(Guid id, string currentPassword, string newPassword)
        {
            try
            {
                var result = await _userService.ChangePasswordAsync(id, currentPassword, newPassword);
                if (result.Succeeded)
                {
                    return Ok("Password changes Successfully.");
                }           
                var errorMessage = result.Errors.FirstOrDefault()?.Description;
                return BadRequest(errorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured while updating user's password with ID: {id}. ");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

   /*     //This delete api is use for check is associated with order or not
        [HttpGet("exist-order/{id}")]
        public async Task<IActionResult> CheckExistOrder(Guid id)
        {
            var Item = await _context.Orders.FindAsync(id);
            if (Item == null)
            {
                return NotFound("User not found.");
            }
            var hasActiveOrders = await _context.Orders
        .AnyAsync(o => o.UserId == id && o.Status == Pending);


            if (hasActiveOrders)
            {
                return BadRequest("This Item is associated with active orders and cannot be deleted.");
            }
            return Ok("true");
        }*/
    }
}
