using GarageOnWheelsAPI.DTOs;
using GarageOnWheelsAPI.Interfaces.IServices;
using GarageOnWheelsAPI.Models.DatabaseModels;
using GarageOnWheelsAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GarageOnWheelsAPI.Controllers  
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly IOtpService _otpService;
        private readonly EmailService _emailService;

        public AuthController(IUserService userService, IAuthService authService, ILogger<AuthController> logger, IOtpService otpService, EmailService emailService)
        {
            _userService = userService;
            _authService = authService;
            _logger = logger;
            _otpService = otpService;
            _emailService = emailService;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            try
            {
                if (await _userService.EmailExistAsync(userDto.Email))
                {
                    return BadRequest("Email already Exist");
                }
                await _authService.Register(userDto);
                return CreatedAtAction(actionName: nameof(UserController.GetUserById), controllerName: "User", routeValues: new { id = userDto.Id }, value: userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Error Occured During Register the User.");
                return BadRequest();
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            try
            {
                var token = await _authService.Login(loginDTO);
                if (token == null)
                {
                    return Unauthorized("Invalid credentials to login.");
                }
                return Ok(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login for user: {Email}.", loginDTO.Email);
                return StatusCode(500, "An error occurred during login. Please try again.");
            }
        }


        [HttpPost("Logout")]
        public async Task<IActionResult> Logout(Guid id)
        {
            try
            {
                await _authService.Logout(id);
                return Ok("Logout Successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging out the user with ID {UserId}.", id);
                return StatusCode(500, "An error occurred during logout. Please try again.");
            }
        }



        [HttpGet("send-otp/{email}")]
        public async Task<IActionResult> SendOtp(string email)
        {
            try
            {
                var user = await _userService.GetEmailAsync(email);
                var otp = await _otpService.GenerateOtpAsync(user.Email);
                await _emailService.SendOtpEmailAsync(email, otp.ToString());
                return Ok("OTP has been sent to your email");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured when sending otp for the user.");
                return BadRequest();
            }

        }
        [HttpPost("Verify-email")]
        public async Task<IActionResult> Verify(string email, string otp)
        {
            try
            {
                var isValid = await _otpService.ValidateOtpAsync(email, Convert.ToInt32(otp));
                if (isValid.Succeeded)
                {
                    var user = await _userService.GetEmailAsync(email);

                    if (user.IsEmailVerified == false)
                    {
                        user.IsEmailVerified = true;
                        var users = UpdateUserDto.mapping(user);
                        await _userService.UpdateUserAsync(users);
                    }

                    return Ok("Otp Verify Successful.");
                }
                return BadRequest("Invalid OTP or OTP expired.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured when logging out the user.");
                return BadRequest();
            }

        }

    }
}
