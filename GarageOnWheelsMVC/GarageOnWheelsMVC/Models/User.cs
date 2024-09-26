using System.ComponentModel.DataAnnotations;

namespace GarageOnWheelsMVC.Models
{
    public class User
    {

        public Guid Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "FirstName is Required")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "LastName is Required")]
        public string LastName { get; set; }
        [Required]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" + @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid Email Format")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Invalid Password Format")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; }
        [Required(ErrorMessage ="Select the Role")]
        public UserRole Role { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Invalid phone number format.")]
        [RegularExpression(@"^(\+91[\-\s]?)?[6-9]\d{9}$", ErrorMessage = "Please enter a valid Phone number.")]
        public string PhoneNo { get; set; }

        [Required]
        public Gender Gender { get; set; }
        [Required(ErrorMessage ="Address must be Required")]
        public string Address { get; set; }
        public bool IsEmailVerified { get; set; } = false;
        public DateTime CreatedDate { get; set; }
        [Required]
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        [Required(ErrorMessage = "Country is required.")]
        public int CountryId { get; set; }

        [Required(ErrorMessage = "State is required.")]
        public int StateId { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public int CityId { get; set; }

        [Required(ErrorMessage = "Area is required.")]
        public int AreaId { get; set; }

        [Required]
        public bool IsDelete { get; set; } = false;


    }

    public enum UserRole
    {
        SuperAdmin = 1,
        GarageOwner = 2,
        Customer = 3
    }


    public enum Gender
    {
        Male = 1,
        Female = 2,
        Others = 3
    }
}
