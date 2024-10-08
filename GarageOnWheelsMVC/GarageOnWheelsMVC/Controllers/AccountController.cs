﻿using GarageOnWheelsMVC.Models.ViewModel;
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
            var model = new RegisterViewModel
            {
                Gender = Gender.Male
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Registration(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (await IsEmailExists(model))
            {
                return View(model);
            }
            var userModel = RegisterViewModel.Mapping(model);


            // Handle profile image upload
            if (model.ProfileImage != null && model.ProfileImage.Length > 0)
            {
                userModel.ProfileImage = await SaveFileAsync(model.ProfileImage);
            }

            // Set CreatedBy to the current user
            userModel.CreatedBy = SessionHelper.GetUserIdFromToken(HttpContext);
            var response = await SendPostRequest<User>("auth/register", userModel);
            if (response.StatusCode == HttpStatusCode.Created)
            {
                await SendOtp(model.Email);
                TempData["Email"] = model.Email;
                return RedirectToAction("VerifyOtp");
            }
            return View(model);
        }

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
                var errors = ModelState.ToDictionary(
                    key => key.Key,
                    val => val.Value.Errors.Select(error => error.ErrorMessage).FirstOrDefault()
                );
                return Json(new { success = false, errors });
            }

            var response = await SendPostRequest<LoginViewModel>("auth/login", model);

            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                var name = SessionHelper.GetUsernameFromToken(token);
                HttpContext.Session.SetString("Token", token);
                HttpContext.Session.SetString("Name", name);

                var cityId = SessionHelper.GetCityFromtoken(token);
                var role = SessionHelper.GetRoleFromToken(token);
                var id = SessionHelper.GetUserIdFromToken(HttpContext);
                var img = SessionHelper.GetImageNameFromToken(token);
                if (img == string.Empty)
                {
                    img = "Unknown.png";
                }
                await SignInUser(role, id, name,img,cityId);

                TempData["Successful"] = "Login Successfully";
                return Json(new { success = true, redirectUrl = Url.Action("Dashboard", "Account") });
            }

            TempData["Message"] = "Invalid login credentials. Please check your email and password.";
            TempData["MessageType"] = "error";

            return Json(new { success = false, message = TempData["Message"] });
            // return Json(new { success = false, message = "Invalid Username or Password." });
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
   
                TempData["Successful"] = "OTP verified successfully.";

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

            // Add a success message to TempData
            TempData["Successful"] = "Logout Successfully";

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


        // Process profile image and return mapped User model
        private User ProcessProfileImage(RegisterViewModel model)
        {
            var user = RegisterViewModel.Mapping(model);
            if (model.ProfileImage != null && model.ProfileImage.Length > 0)
            {
                var uniqueId = Guid.NewGuid().ToString();
                var fileExtension = Path.GetExtension(model.ProfileImage.FileName);
                var fileName = $"{model.FirstName}_{uniqueId}{fileExtension}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Images", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfileImage.CopyTo(fileStream);
                }

                user.ProfileImage = fileName;
            }

            return user;
        }


        private async Task SignInUser(string role, Guid id, string unique_name,string img,string cityId)
        {   
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Role, role),
        new Claim(ClaimTypes.NameIdentifier, id.ToString()),
        new Claim(ClaimTypes.Name, unique_name),
        new Claim("profileImg",img),
        new Claim("cityId", cityId)
    };
      
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Sign in the user
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }



        // Remote validation
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsEmailAvailable(string email)
        {
            var emailExists = await CheckIfExists($"user/search?email={email}");

            if (emailExists)
            {
                return Json($"{email} is already taken.");
            }

            return Json(true);
        }

        //Save file
        private async Task<string> SaveFileAsync(IFormFile file)
        {
            var uniqueId = Guid.NewGuid().ToString();
            var fileExtension = Path.GetExtension(file.FileName);
            var fileName = $"{uniqueId}{fileExtension}";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Images", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }
        //update File and save
        private async Task<string> SaveUpdatedFileAsync(string existingFileName, IFormFile updateImg)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Images");
            if (!string.IsNullOrEmpty(existingFileName))
            {
                var existingFilePath = Path.Combine(path, existingFileName);
                if (System.IO.File.Exists(existingFilePath))
                {
                    System.IO.File.Delete(existingFilePath);
                }
            }

            return await SaveFileAsync(updateImg);
        }
    }
}
