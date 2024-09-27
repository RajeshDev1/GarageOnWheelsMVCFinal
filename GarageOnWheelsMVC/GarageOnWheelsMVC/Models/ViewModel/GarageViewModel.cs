using System.ComponentModel.DataAnnotations;

namespace GarageOnWheelsMVC.Models.ViewModel
{
    public class GarageViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage ="Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage ="Select a Garage Owner")]
        public Guid UserId { get; set; }
        [Required(ErrorMessage ="Address is required")]
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
        [Required(ErrorMessage = "Country is required.")]
        public int CountryId { get; set; }

        [Required(ErrorMessage = "State is required.")]
        public int StateId { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public int CityId { get; set; }

        [Required(ErrorMessage = "Area is required.")]
        public int AreaId { get; set; }
        public  Country? Country { get; set; }
        public  City? City { get; set; }
        public  State? State { get; set; }
        public  Area? Area { get; set; }

        public string? GarageOwnerName { get; set; }
    }
}
