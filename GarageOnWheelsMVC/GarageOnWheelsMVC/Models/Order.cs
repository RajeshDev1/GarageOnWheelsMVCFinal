using System.ComponentModel.DataAnnotations;

namespace GarageOnWheelsMVC.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required(ErrorMessage ="Garage is required")]
        public Guid GarageId { get; set; }
        [Required(ErrorMessage ="Write the required services")]
        public string ServiceDetails { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }
        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public DateTime UpdatedDate { get; set; }
        [Required]
        public Guid CreatedBy { get; set; }
        [Required]
        public Guid UpdatedBy { get; set; }
        [Required]
        public bool IsDelete { get; set; } = false;


    }

    public enum OrderStatus
    {
        Pending = 1,
        Completed = 2,
    }
}
