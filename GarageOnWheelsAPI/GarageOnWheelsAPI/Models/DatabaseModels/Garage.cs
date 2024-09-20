using GarageOnWheelsAPI.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarageOnWheelsAPI.Models.DatabaseModels
{
    public class Garage
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } 
        public Guid UserId { get; set; }
        public int AreaId { get; set; }
        public int CityId { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string Address { get; set; } 
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsDelete { get; set; } = false;
        [ForeignKey("UserId")]        
        public virtual User User { get; set; }
        [ForeignKey("CountryId")]
        public virtual Country Country { get; set; }
        [ForeignKey("StateId")]
        public virtual State State { get; set; }     
        [ForeignKey("CityId")]
        public virtual City City { get; set; }
        [ForeignKey("AreaId")]
        public virtual Area Area { get; set; }

        public virtual ICollection<Orders> Orders { get; set; }


    }
}
