using System;
using System.ComponentModel.DataAnnotations;

namespace GarageOnWheelsAPI.DTOs
{
    public class OrderCreateDto
    {
        [Required]
        public Guid GarageId { get; set; } // The ID of the garage where the order is being placed

        [Required]
        [StringLength(100, ErrorMessage = "Service name cannot exceed 100 characters.")]
        public string ServiceName { get; set; } // The name of the service being requested

        [Required]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } // A brief description of the order

        [Required]
        public DateTime ServiceDate { get; set; } // The date and time when the service is scheduled

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "The price must be greater than 0.")]
        public decimal Price { get; set; } // The price of the service

        [Required]
        public Guid CustomerId { get; set; } // The ID of the customer placing the order

        // Optional: Additional fields can be added based on your application's requirements
    }
}
