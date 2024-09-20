using GarageOnWheelsAPI.Interfaces.IServices;
using GarageOnWheelsAPI.Interfaces.Repositories;
using GarageOnWheelsAPI.Models.DatabaseModels;
using GarageOnWheelsAPI.Repositories;
using Microsoft.AspNetCore.Identity;

namespace GarageOnWheelsAPI.Services
{
    public class OtpService : IOtpService
    {
        private readonly IOtpRepository _otpRepository;
        private readonly IOtpService _otpService;
        private readonly TimeSpan _otpValidityDuration = TimeSpan.FromMinutes(5);

        public OtpService(IOtpRepository otpRepository)
        {
            _otpRepository = otpRepository;
        }

        public async Task<int> GenerateOtpAsync(string email)
        {
            var random = new Random();
            int otpCode = random.Next(100000, 999999);

            var otp = new Otp
            {
                Id = Guid.NewGuid(),
                Email = email,
                Code = otpCode,
                ExpireTime = DateTime.Now.Add(_otpValidityDuration)
            };

            // Save the OTP to the database
            await _otpRepository.CreateOtpAsync(otp);
            return otpCode;
        }

        public async Task<IdentityResult> ValidateOtpAsync(string email, int otpCode)
        {
            var otp = await _otpRepository.GetOtpByUserIdAsync(email);
            if (otp == null || otp.Code != otpCode)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Otp not matched." });
            }

            if (otp.ExpireTime < DateTime.UtcNow)
            {

                await _otpRepository.DeleteOtpAsync(otp);
                return IdentityResult.Failed(new IdentityError { Description = "Otp is expired.." });
            }
            await _otpRepository.DeleteOtpAsync(otp);
            return IdentityResult.Success;
        }
    }
}
