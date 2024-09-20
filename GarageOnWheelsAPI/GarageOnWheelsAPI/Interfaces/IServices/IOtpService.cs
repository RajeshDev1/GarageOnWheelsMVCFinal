using Microsoft.AspNetCore.Identity;

namespace GarageOnWheelsAPI.Interfaces.IServices
{
    public interface IOtpService
    {
        Task<int> GenerateOtpAsync(string email);
        Task<IdentityResult> ValidateOtpAsync(string email,int otpCode);
    }
}
