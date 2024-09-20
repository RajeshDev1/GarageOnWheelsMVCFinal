namespace GarageOnWheelsAPI.DTOs
{
    public class RevenueReportDto
    {
        public decimal TotalRevenue { get; set; }
        public DateTime ReportDate { get; set; }
        public string GarageName { get; set; } 
        public Guid GarageId { get; set; }  
    }
}
