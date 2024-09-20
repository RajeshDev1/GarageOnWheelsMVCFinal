using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GarageOnWheelsAPI.Models.DatabaseModels
{
    public class Area
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int CityId { get; set; }
        [ForeignKey("CityId")]
        [JsonIgnore]
        public virtual City City { get; set; }

    }
}
