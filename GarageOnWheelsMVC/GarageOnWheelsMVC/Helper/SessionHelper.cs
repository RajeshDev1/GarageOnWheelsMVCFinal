using System.IdentityModel.Tokens.Jwt;

namespace GarageOnWheelsMVC.Helper
{
    public class SessionHelper
    {
        public static Guid GetUserIdFromToken(HttpContext httpcontext)
        {
            string token = httpcontext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
            {
                return Guid.Empty;
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return new Guid(jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value ?? Guid.Empty.ToString());
        }

        public static string GetRoleFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var role = jwtToken.Claims.FirstOrDefault(c => c.Type == "role");
            return role?.Value;
        }

        public static string GetUsernameFromSession(HttpContext httpContext)
        {
            return httpContext.Session.GetString("Username") ?? string.Empty;
        }

             
        public static string GetUsernameFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var username = jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name");
            return username?.Value ?? string.Empty;
        }

        public static string GetImageNameFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var img = jwtToken.Claims.FirstOrDefault(c => c.Type == "profileimg");
            return img?.Value;
        }

    }
}
