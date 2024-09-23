using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GarageOnWheelsAPI.DTOs;
using GarageOnWheelsAPI.Enums;
using GarageOnWheelsAPI.Models.DatabaseModels;

namespace GarageOnWheelsAPI.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<string> Login(LoginDTO loginDTO);
        Task Logout(Guid id);
        Task Register(UserDto userDto);
    }
}
