using GarageOnWheelsMVC.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Linq;
using GarageOnWheelsMVC.Models;
using System.Reflection;
using GarageOnWheelsMVC.Helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Data;
using System.Net.Http.Headers;

namespace GarageOnWheelsMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        string baseurl = "https://localhost:7107/api/";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public AccountController(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public void SetAuthorize(HttpContext httpContext)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");
            }
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }


        [HttpGet]
        private async Task<bool> CheckIfExists(string query)
        {
            var response = await _httpClient.GetAsync($"{baseurl}{query}");
            return JsonSerializer.Deserialize<bool>(await response.Content.ReadAsStringAsync());
        }

        [HttpGet]
        private async Task<bool> IsEmailExists(RegisterViewModel model)
        {


            var emailExists = await CheckIfExists($"user/search?email={model.Email}");
            if (emailExists)
            {
                ModelState.AddModelError("Email", "Email Already Exist.");
            }
            return emailExists;
        }


        [HttpGet]
        public async Task<IActionResult> Registration()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Registration(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Check if username or email already exists
            if (await IsEmailExists(model))
            {
                return View(model);
            }

            // Process profile image and map to User model
            var userModel = RegisterViewModel.Mapping(model);

            var response = await SendPostRequest<User>("auth/register", userModel);
            if (response.StatusCode == HttpStatusCode.Created)
            {
                await SendOtp(model.Email);
                TempData["Email"] = model.Email;
                return RedirectToAction("VerifyOtp");
            }
            return View(model);
        }


        //Helper method to send post request
        private async Task<HttpResponseMessage> SendPostRequest<T>(string endpoint, T model)
        {
            var jsonModel = JsonSerializer.Serialize(model);
            var content = new StringContent(jsonModel, Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync($"{baseurl}{endpoint}", content);
        }

        [HttpGet]
        public IActionResult Login()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Call the Web API to authenticate the user
            var response = await SendPostRequest<LoginViewModel>("auth/login", model);

            if (response.IsSuccessStatusCode)
            {
                // If the response is successful, retrieve the token
                var token = await response.Content.ReadAsStringAsync();

                // Extract details from the token
                var name = SessionHelper.GetUsernameFromToken(token);
                HttpContext.Session.SetString("Token", token);
                HttpContext.Session.SetString("Name", name);

                var role = SessionHelper.GetRoleFromToken(token);
                var id = SessionHelper.GetUserIdFromToken(HttpContext);

                // Perform sign-in logic    
                await SignInUser(role, id, name);

                // when Success then go to Next Page
                TempData["Successful"] = "Login Successfully";

                // Redirect to the dashboard after successful login
                return RedirectToAction("Dashboard", "Account");
            }

            // Handle Unauthorized (invalid credentials)
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                TempData["Message"] = "Invalid login credentials. Please check your email and password.";
                TempData["MessageType"] = "error";  // Message type for Toaster
            }
            // Handle BadRequest (e.g., missing required fields)
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                TempData["Message"] = "Invalid request. Please check your input and try again.";
                TempData["MessageType"] = "warning";
            }
            // Handle other error cases
            else
            {
                TempData["Message"] = "An error occurred while processing your request. Please try again later.";
                TempData["MessageType"] = "error";
            }

            // Return the same view with the model, so it retains the user's input
            return View(model);
        }



        [HttpGet]
        public IActionResult VerifyOtp()
        {
            TempData.Keep("Email");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(OtpVerificationViewModel model)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{baseurl}auth/verify-email?email={model.Email}&otp={model.OTP}");
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ModelState.AddModelError("", "Invalid OTP. Please try again.");
            }
            TempData.Keep("Email");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var request = new HttpRequestMessage(HttpMethod.Post, $"{baseurl}auth/logout?id={id}");

            var response = await _httpClient.SendAsync(request);
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        private async Task SendOtp(string email)
        {
            var otpsendResponse = await _httpClient.GetAsync($"{baseurl}auth/send-otp/{email}");
            if (!otpsendResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to send OTP. Please try again.");
            }
        }


        private async Task SignInUser(string role, Guid id, string unique_name)
        {
            // Create a list of claims including the unique_name
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Role, role),
        new Claim(ClaimTypes.NameIdentifier, id.ToString()),
        new Claim(ClaimTypes.Name, unique_name)
    };

            // Create a ClaimsIdentity and ClaimsPrincipal
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Sign in the user
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }




    }
}
