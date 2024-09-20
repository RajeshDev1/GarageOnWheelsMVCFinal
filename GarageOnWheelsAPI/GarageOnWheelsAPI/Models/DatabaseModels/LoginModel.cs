using System.ComponentModel.DataAnnotations;

namespace GarageOnWheelsAPI.Models.DatabaseModels
{
    public class LoginModel
    {
        public string Email { get; set; } 
        public string Password { get; set; } 
    }
}
