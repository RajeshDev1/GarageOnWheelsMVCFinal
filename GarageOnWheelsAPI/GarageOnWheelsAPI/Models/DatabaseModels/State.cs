using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GarageOnWheelsAPI.Models.DatabaseModels
{
    public class State
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int CountryId { get; set; }

        [ForeignKey("CountryId")]
        [JsonIgnore]
        public virtual Country Country { get; set; }
        [JsonIgnore]
        public virtual ICollection<City> Cities { get; set; } 
  
    }
}
