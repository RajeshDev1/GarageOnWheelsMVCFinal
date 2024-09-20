using System;

namespace GarageOnWheelsAPI.DTOs.Order
{
    public class OrderRequestDto
    {
        // Unique identifier for the order
        public Guid GarageId { get; set; }

        // Unique identifier for the customer placing the order
        public Guid UserId { get; set; }

        // Details of the order
        public string ServiceDetails { get; set; } 

        // Status of the order (e.g., Pending, Completed)
        public string Status { get; set; } = "Pending";

        // Date and time when the order was placed
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    }
}
