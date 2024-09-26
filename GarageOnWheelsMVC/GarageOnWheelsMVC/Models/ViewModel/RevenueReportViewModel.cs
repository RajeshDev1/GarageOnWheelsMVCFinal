using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GarageOnWheelsMVC.Models.ViewModel
{
    public class RevenueReportViewModel
    {
        [Required(ErrorMessage ="Garage is required")]
        public Guid GarageId { get; set; }
        [Required(ErrorMessage = "Garage Name is required")]
        public string GarageName { get; set; }
        public DateTime ReportDate { get; set; }
        public decimal TotalRevenue { get; set; }
        [Required(ErrorMessage = "Start Date is required")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "End Date is required")]
        public DateTime EndDate { get; set; }
        public IEnumerable<SelectListItem> Garages { get; set; }
    }
}
