using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GarageOnWheelsAPI.Models.DatabaseModels
{
    public class Country
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public virtual ICollection<State> States { get; set; }

    }
}
