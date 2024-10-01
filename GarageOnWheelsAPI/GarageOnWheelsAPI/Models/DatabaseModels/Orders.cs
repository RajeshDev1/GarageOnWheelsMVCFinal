using GarageOnWheelsAPI.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarageOnWheelsAPI.Models.DatabaseModels
{
    public class Orders
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }  
        public Guid GarageId { get; set; } 
        public string ServiceDetails { get; set; } 
        public DateTime OrderDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int Status { get; set; }
        public bool IsDelete { get; set; } = false;
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("GarageId")]
        public virtual Garage Garage { get; set; }
        public string? ImageUploadByCustomer { get; set; }
    }

}

