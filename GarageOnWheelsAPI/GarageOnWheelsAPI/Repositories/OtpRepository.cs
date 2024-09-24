using GarageOnWheelsAPI.Data;
using GarageOnWheelsAPI.Interfaces.Repositories;
using GarageOnWheelsAPI.Models.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace GarageOnWheelsAPI.Repositories
{
    public class OtpRepository : IOtpRepository
    {
        private readonly ApplicationDbContext _context;

        public OtpRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateOtpAsync(Otp otp)
        {
            await _context.AddAsync(otp);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOtpAsync(Otp otp)
        {
             _context.Remove(otp);
            await _context.SaveChangesAsync();
        }

        public async Task<Otp> GetOtpByUserIdAsync(string email)
        {
            return await _context.Otps.FirstOrDefaultAsync(o => o.Email == email);
        }
    }
}
