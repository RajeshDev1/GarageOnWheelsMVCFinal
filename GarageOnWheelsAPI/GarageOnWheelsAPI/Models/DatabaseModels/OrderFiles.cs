using System.ComponentModel.DataAnnotations.Schema;

namespace GarageOnWheelsAPI.Models.DatabaseModels
{
    public class OrderFiles
    {
        public Guid Id { get; set; }

        [ForeignKey("Order")]
        public Guid OrderId { get; set; }
        public string? FileName { get; set; }
        public DateTime UploadDate { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

    }
}
