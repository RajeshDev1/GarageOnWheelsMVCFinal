using GarageOnWheelsAPI.Models.DatabaseModels;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarageOnWheelsAPI.Models.DatabaseModels
{
    public class City
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int StateId { get; set; }
        [ForeignKey("StateId")]
        [JsonIgnore]
        public virtual State State { get; set; }
        [JsonIgnore]
        public virtual ICollection<Area> Areas { get; set; }
    }
}
