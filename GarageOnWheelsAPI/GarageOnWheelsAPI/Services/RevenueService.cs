using System;
using System.Linq;
using System.Threading.Tasks;
using GarageOnWheelsAPI.DTOs;
using GarageOnWheelsAPI.Interfaces.IServices;
using GarageOnWheelsAPI.Interfaces.Repositories;
using GarageOnWheelsAPI.Models.DatabaseModels;

namespace GarageOnWheelsAPI.Services
{
    public class RevenueService : IRevenueService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IGarageRepository _garageRepository;

        public RevenueService(IOrderRepository orderRepository, IGarageRepository garageRepository)
        {
            _orderRepository = orderRepository;
            _garageRepository = garageRepository;
        }

        public async Task<IEnumerable<RevenueReportDto>> GetRevenueReportByDateRangeAsync(Guid garageId, DateTime startDate, DateTime endDate)
        {
            var orders = await _orderRepository.GetOrdersByGarageIdAsync(garageId);
            var garage = await _garageRepository.GetGarageByIdAsync(garageId);

            var filteredOrders = orders
                .Where(order => order.UpdatedDate.Date >= startDate.Date && order.UpdatedDate.Date <= endDate.Date)
                .ToList();

            var groupedOrders = filteredOrders
                .GroupBy(order => order.UpdatedDate.Date)
                .Select(group => new RevenueReportDto
                {
                    TotalRevenue = group.Sum(order => order.TotalAmount),
                    ReportDate = group.Key,
                    GarageName = garage?.Name ?? "Unknown Garage",
                    GarageId = garageId
                })
                .OrderByDescending(report => report.ReportDate).ToList();


            return groupedOrders;
        }

    }
}
