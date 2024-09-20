using System.ComponentModel.DataAnnotations;

namespace GarageOnWheelsMVC.Models.ViewModel
{
    public class OtpVerificationViewModel
    {
        public string Email { get; set; }

        [Required(ErrorMessage = "OTP is required")]
        public string OTP { get; set; }
    }
}