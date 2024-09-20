using GarageOnWheelsAPI.Models.DatabaseModels;

namespace GarageOnWheelsAPI.Interfaces.Repositories
{
    public interface IOtpRepository
    {
        Task CreateOtpAsync(Otp otp);
        Task DeleteOtpAsync(Otp otp);
        Task<Otp> GetOtpByUserIdAsync(string email);

    }
}
