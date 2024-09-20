using Microsoft.AspNetCore.Mvc.Rendering;

namespace GarageOnWheelsMVC.Models.ViewModel
{
    public class RevenueReportViewModel
    {
        public Guid GarageId { get; set; }
        public string GarageName { get; set; }
        public DateTime ReportDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }

        // For the view's form inputs
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<SelectListItem> Garages { get; set; }
    }
}
