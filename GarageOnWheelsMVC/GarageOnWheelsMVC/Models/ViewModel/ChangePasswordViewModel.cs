using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace GarageOnWheelsMVC.Models.ViewModel
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Old Password is required.")]
        [DisplayName("Old Password")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "New Password is required.")]
        [DisplayName("New Password")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DisplayName("Confirm Password")]
        [Compare("NewPassword", ErrorMessage = "Confirm Password does not match the New Password.")]
        public string ConfirmPassword { get; set; }

    }
}
