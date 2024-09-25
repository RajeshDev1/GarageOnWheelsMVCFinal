using GarageOnWheelsAPI.Enums;
using GarageOnWheelsAPI.Models.DatabaseModels;
using System.ComponentModel.DataAnnotations;

namespace GarageOnWheelsAPI.DTOs
{
    public class UserDto
    {           
                    public Guid Id { get; set; }
                    [Required]
                    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
                    public string FirstName { get; set; }
                    [Required]
                    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters.")]
                    public string LastName { get; set; }
                    [Required]                 
                    [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" + @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid Email Format")]
                    public string Email { get; set; }
                    [Required]
                    [DataType(DataType.Password)]
                     [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Invalid Password Format")]
                    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
                    public string Password { get; set; }
                    [Required(ErrorMessage ="Select a Role")]
                    public UserRole Role { get; set; }

                    [Required(ErrorMessage = "Phone number is required.")]
                    [DataType(DataType.PhoneNumber, ErrorMessage = "Invalid phone number format.")]
                    [RegularExpression(@"^(\+91[\-\s]?)?[6-9]\d{9}$", ErrorMessage = "Please enter a valid Indian phone number.")]
                    public string PhoneNo { get; set; }

                    [Required(ErrorMessage ="Select a Gender")]
                    public Gender Gender { get; set; } = Gender.Male;
                    [Required]
                    public string Address { get; set; }
                    public bool IsEmailVerified { get; set; }=false;
                    public DateTime CreatedDate { get; set; }
                    [Required]
                    public Guid CreatedBy { get; set; }
                    public Guid UpdatedBy { get; set; }
                    public DateTime UpdatedDate { get; set; }
                    [Required]
                    public int CountryId { get; set; }
                    [Required]
                    public int StateId { get; set; }
                    [Required]
                    public int CityId { get; set; }
                    [Required]
                    public int AreaId { get; set; }

                    [Required]
                    public bool IsDelete { get; set; } = false;



        public static User mapping(UserDto userDto)
        {
            return new User()
            {
                Id = userDto.Id,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                PasswordHash = userDto.Password,
                Role = (int)userDto.Role,
                Address = userDto.Address,
                PhoneNo = userDto.PhoneNo,
                IsEmailVerified = userDto.IsEmailVerified,
                CreatedDate = userDto.CreatedDate,
                UpdatedDate = userDto.UpdatedDate,
                AreaId = userDto.AreaId,
                CountryId = userDto.CountryId,
                StateId = userDto.StateId,
                CityId = userDto.CityId,
                Gender = Enum.GetName(typeof(Gender),userDto.Gender),
                IsDelete = userDto.IsDelete,
                CreatedBy = userDto.CreatedBy,
                UpdatedBy = userDto.UpdatedBy,
            };

        }

        public static UserDto mapping(User user)
        {
            return new UserDto()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.PasswordHash,
                Role = (UserRole)user.Role,
                Address = user.Address,
                PhoneNo = user.PhoneNo,
                Gender = (Gender)Enum.Parse(typeof(Gender), user.Gender),
                IsEmailVerified = user.IsEmailVerified,
                CreatedDate = user.CreatedDate,
                UpdatedDate = user.UpdatedDate,
                AreaId = user.AreaId,
                CountryId = user.CountryId,
                StateId = user.StateId,
                CityId = user.CityId,
                IsDelete = user.IsDelete,
                CreatedBy = user.CreatedBy,
                UpdatedBy = user.UpdatedBy,
            };

        }

        public static IEnumerable<UserDto> mapping(IEnumerable<User> users)
        {
            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                userDtos.Add(mapping(user));
            }
            return userDtos;
        }

        public static IEnumerable<User> mapping(IEnumerable<UserDto> userDto)
        {
            var userList = new List<User>();
            foreach (var user in userDto)
            {
                userList.Add(mapping(user));
            }
            return userList;
        }


    }
}
