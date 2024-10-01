using GarageOnWheelsAPI.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarageOnWheelsAPI.Models.DatabaseModels
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; } 
        public string Email { get; set; } 
        public string PasswordHash { get; set; } 
        public int Role { get; set; }
        public string PhoneNo { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public int AreaId { get; set; }
        public string Address { get; set; } 
        public string? Token { get; set; } 
        public bool IsEmailVerified { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Gender { get; set; }
        public bool IsDelete { get; set; } = false;
        [ForeignKey("CountryId")]
        public virtual Country Country{ get; set; }     
        [ForeignKey("StateId")]
        public virtual State State { get; set; }     

        [ForeignKey("CityId")]
        public virtual City City {  get; set; }       
        public string? ProfileImage { get; set; }

        [ForeignKey("AreaId")]
        public virtual Area Area { get; set; }

        public virtual ICollection<Garage> Garages { get; set; }

        public virtual ICollection<Orders> Orders { get; set; }

    }

}

