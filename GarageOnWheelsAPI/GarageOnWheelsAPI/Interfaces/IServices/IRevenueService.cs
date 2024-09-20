using GarageOnWheelsAPI.DTOs;

namespace GarageOnWheelsAPI.Interfaces.IServices
{
    public interface IRevenueService
    { 
        // Get revenue report for a specific garage
        Task<IEnumerable<RevenueReportDto>> GetRevenueReportByDateRangeAsync(Guid garageId, DateTime startDate, DateTime endDate);

 /*       // Get all revenue reports for a specific date range
        Task<IEnumerable<RevenueReportDto>> GetAllRevenueReportsAsync(DateTime startDate, DateTime endDate);

        // Get revenue report for a specific garage on a specific date
        Task<RevenueReportDto> GetRevenueReportByDateAsync(Guid garageId, DateTime date);*/

    }
}
