using System.ComponentModel.DataAnnotations;

namespace GarageOnWheelsMVC.Models.ViewModel
{
    public class GarageViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage ="Name is Required")]
        public string Name { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required(ErrorMessage ="Address is Required")]
        public string Address { get; set; }
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
        [Required]
        public int CountryId { get; set; }
        [Required]
        public int StateId { get; set; }
        [Required]
        public int CityId { get; set; }
        [Required]
        public int AreaId { get; set; }
        public  Country? Country { get; set; }
        public  City? City { get; set; }
        public  State? State { get; set; }
        public  Area? Area { get; set; }

        public string? GarageOwnerName { get; set; }
    }
}
