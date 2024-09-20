using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using GarageOnWheelsAPI.Interfaces.IServices;
using GarageOnWheelsAPI.Interfaces.Repositories;
using GarageOnWheelsAPI.Models.DatabaseModels;
using GarageOnWheelsAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Linq;
using GarageOnWheelsAPI.DTOs;
using GarageOnWheelsAPI.Utils;

namespace GarageOnWheelsAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly JwtTokenGenerator _jwtTokenGenerator;

        public AuthService(
            IUserRepository userRepository,
            IConfiguration configuration,
            JwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<string> Login(LoginDTO loginDTO)
        {                                   
           
           var hashPassword = PasswordHasher.HashPassword(loginDTO.Password);
            var user = await _userRepository.getUserEmailAsync(loginDTO.Email, hashPassword);

            if (user != null )
            {
                var token = _jwtTokenGenerator.GenerateToken(user);
                user.Token = token;
             await _userRepository.UpdateUserAsync(user);
                return token;
            }
            else return null;
        }

        public async Task Logout(Guid id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            user.Token = null;
            await _userRepository.UpdateUserAsync(user);
        }

        public async Task Register(UserDto userDto)
        {
            userDto.Id = Guid.NewGuid();
            userDto.CreatedDate = DateTime.Now;
            userDto.UpdatedDate = userDto.CreatedDate;
            userDto.CreatedBy = userDto.Id;
            userDto.UpdatedBy = userDto.CreatedBy;
            userDto.Password = PasswordHasher.HashPassword(userDto.Password);

            var user = UserDto.mapping(userDto);

            await _userRepository.AddUserAsync(user);
        }
    }
}
