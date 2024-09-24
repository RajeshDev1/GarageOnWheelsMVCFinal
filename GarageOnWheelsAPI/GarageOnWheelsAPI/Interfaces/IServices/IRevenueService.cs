using GarageOnWheelsAPI.DTOs;

namespace GarageOnWheelsAPI.Interfaces.IServices
{
    public interface IRevenueService
    { 
        Task<IEnumerable<RevenueReportDto>> GetRevenueReportByDateRangeAsync(Guid garageId, DateTime startDate, DateTime endDate);
    }
}
