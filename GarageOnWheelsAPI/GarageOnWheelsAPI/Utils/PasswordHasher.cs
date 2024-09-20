using AutoMapper.Configuration.Conventions;

namespace GarageOnWheelsAPI.Utils
{
    public  class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedbyte = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedbyte).Replace("-", "0").ToLower();
            }
        }
    }
}
