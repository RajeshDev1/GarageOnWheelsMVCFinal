namespace GarageOnWheelsAPI.DTOs
{
    public class ResetPasswordDTO
    {
        public string Email { get; set; }
        public int OtpCode { get; set; }
        public string NewPassword { get; set; }
    }
}
