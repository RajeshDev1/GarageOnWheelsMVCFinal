using GarageOnWheelsMVC.Helper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace GarageOnWheelsMVC.Models.ViewModel
{
    public class RegisterViewModel
    {

        public Guid Id { get; set; }
        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First Name must be between 2 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s\(\)]+$", ErrorMessage = "First Name must contain only alphabets.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last Name must be between 2 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s\(\)]+$", ErrorMessage = "Last Name must contain only alphabets.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [Remote(action: "IsEmailAvailable", controller: "Account", ErrorMessage = "Email is already in use.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Password must have at least 8 characters, including one uppercase letter, one lowercase letter, one number and one special character.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; }

        [Required]
        public UserRole Role { get; set; } = UserRole.Customer;

        [Required(ErrorMessage = "Phone No is required.")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Invalid phone number format.")]
        [RegularExpression(@"^(\+91[\-\s]?)?[6-9]\d{9}$", ErrorMessage = "Please enter a valid Phone number.")]
        public string PhoneNo { get; set; }


        [Required(ErrorMessage = "Gender is required.")]
        public Gender Gender { get; set; } = Gender.Male;

        [Required(ErrorMessage = "Address is required.")]
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
        /*public bool? IsProfileEdit { get; set; }*/

        [AllowedFileExtensions(new[] { ".jpg", ".jpeg", ".png", ".gif" }, ErrorMessage = "Please upload a valid image file (jpg, jpeg, png, gif).")]
        public IFormFile? ProfileImage { get; set; }
        public string? image { get; set; }
        public static User Mapping(RegisterViewModel model)
        {
            return new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                Role = model.Role,
                PhoneNo = model.PhoneNo,
                Address = model.Address,
                Gender = model.Gender,
                CountryId = model.CountryId,
                StateId = model.StateId,
                CityId = model.CityId,
                AreaId = model.AreaId,
                CreatedBy = model.CreatedBy,
                IsDelete = model.IsDelete,
                UpdatedDate = model.UpdatedDate,
                UpdatedBy = model.UpdatedBy,             
               

            };
        }

        public static RegisterViewModel Mapping(User user)
        {
            return new RegisterViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
                Role = user.Role,
                PhoneNo = user.PhoneNo,
                Address = user.Address,
                Gender = user.Gender,
                CountryId = user.CountryId,
                StateId = user.StateId,
                CityId = user.CityId,
                AreaId = user.AreaId,
                CreatedBy = user.CreatedBy,
                IsDelete = user.IsDelete,
                CreatedDate = user.CreatedDate,
                image = user.ProfileImage
            };
        }

        public RegisterViewModel()
        {
            Gender = Enum.GetValues(typeof(Gender)).Cast<Gender>().First();
        }
    }
}
