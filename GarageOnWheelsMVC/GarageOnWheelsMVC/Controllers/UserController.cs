using GarageOnWheelsMVC.Helper;
using GarageOnWheelsMVC.Models;
using GarageOnWheelsMVC.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http.Headers;
using NuGet.Common;
using Microsoft.AspNetCore.Authorization;


namespace GarageOnWheelsMVC.Controllers
{
    public class UserController : Controller
    {
        private readonly ApiHelper _apiHelper;
        private readonly string baseUrl;


        public UserController(ApiHelper apiHelper, IConfiguration configuration)
        {       
            _apiHelper = apiHelper;
            baseUrl = configuration["AppSettings:BaseUrl"];
        }
        public IActionResult Dashboard()
        {
            return View();
        }


        //Get All User
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetAllUsers(int page = 1, int pageSize = 3)
        {
            var users = await _apiHelper.GetAsync<List<User>>("user/all",HttpContext);
            if (users == null)
            {
                return BadRequest("Error occurs during fetch user ");
            }

            var totalCount = users.Count;

            // Get the paginated list
            var paginatedUsers = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new UserListViewModel
            {
                Users = paginatedUsers,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    TotalItems = totalCount,
                    ItemsPerPage = pageSize
                }
            };
            return View(viewModel);
        }

        //Get All Customer
        [Authorize(Roles = "GarageOwner")]
        public async Task<IActionResult> GetAllCustomers(int page = 1, int pageSize = 3)
        {
            var users = await _apiHelper.GetAsync<List<User>>("user/allCustomer", HttpContext);
            if (users == null)
            {
                return BadRequest("Error occurs during fetch Customers ");
            }

            var totalCount = users.Count;

            // Get the paginated list
            var paginatedUsers = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new UserListViewModel
            {
                Users = paginatedUsers,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    TotalItems = totalCount,
                    ItemsPerPage = pageSize
                }
            };
            return View(viewModel);
        }
        //Ge all GarageOwner
        [Authorize(Roles = "SuperAdmin")]
        public async Task<JsonResult> GetAllGarageOwners()
        {
            var users = await _apiHelper.GetAsync<List<User>>("user/allgarageowner", HttpContext);
            return new JsonResult(users ?? new List<User>());
        }

        //Get Users By Role
        [Authorize(Roles = "SuperAdmin")]
        public async Task<JsonResult> GetUsersByRole()
        {
            var users = await _apiHelper.GetAsync<List<User>>("user/by-role?role=GarageOwner", HttpContext);
            return new JsonResult(users ?? new List<User>());
        }

       
        // Get Form to Create the User
        [Authorize(Roles = "SuperAdmin,GarageOwner")]
        public IActionResult Create()
        {
            return View();
        }


        // POST: Handle New user Creation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if email exists
            if (await IsEmailExists(model.Email))
            {
                return View(model);
            }

            var userModel = RegisterViewModel.Mapping(model);
            userModel.CreatedBy = SessionHelper.GetUserIdFromToken(HttpContext);
            var response = await _apiHelper.SendPostRequest("user/create", userModel,HttpContext);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                await _apiHelper.SendOtp(model.Email,HttpContext);
                TempData["Email"] = model.Email;
                return RedirectToAction("VerifyOtp");
            }

            return View(model);
        }

        public IActionResult VerifyOtp()
        {
            TempData.Keep("Email");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(OtpVerificationViewModel model)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"auth/verify-email?email={model.Email}&otp={model.OTP}");
            var response = await _apiHelper.SendJsonAsync(request.RequestUri.ToString(), model, HttpMethod.Post,HttpContext);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login", "Account");
            }

            ModelState.AddModelError("", "Invalid OTP. Please try again.");
            TempData.Keep("Email");
            return View(model);
        }
        // Edit the User
        [Authorize]
        public async Task<IActionResult> Edit(Guid id)
        {
            var user = await _apiHelper.GetAsync<User>($"user/{id}", HttpContext);
            if (user == null)
            {
                return NotFound();
            }

            var userViewModel = UpdateUserViewModel.mapping(user);
            return View(userViewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(UpdateUserViewModel model, string? previousUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.previousurl = Request.Headers["Referer"].ToString();
                return View(model);
            }

            model.UpdatedBy = SessionHelper.GetUserIdFromToken(HttpContext);
            var response = await _apiHelper.SendJsonAsync($"user/update/{model.Id}", model, HttpMethod.Put, HttpContext);

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                TempData["Successful"] = "User successfully updated!";
                return !string.IsNullOrEmpty(previousUrl) ? Redirect(previousUrl) : RedirectToAction("GetAllUsers");
            }

            return View(model);
        }

        //Edit Profile
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditProfile(Guid id)
        {
            var user = await _apiHelper.GetAsync<User>($"user/{id}", HttpContext);
            if (user == null)
            {
                return NotFound();
            }

            var userViewModel = UpdateUserViewModel.mapping(user);
            return View(userViewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditProfile(UpdateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.UpdatedBy = SessionHelper.GetUserIdFromToken(HttpContext);
            var response = await _apiHelper.SendJsonAsync($"user/update/{model.Id}", model, HttpMethod.Put, HttpContext);

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return RedirectToAction("Dashboard", "Account");
            }

            return View(model);
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            var endpoint = $"{baseUrl}user/delete/{id}"; 
            var response = await _apiHelper.DeleteAsync(endpoint, HttpContext); 

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                TempData["Successful"] = "User successfully deleted!";
                return RedirectToAction("GetAllUsers");
            }
            return BadRequest($"Failed to delete the user. Status Code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
        }

        private async Task<bool> IsEmailExists(string email)
        {
            var emailExists = await _apiHelper.CheckIfExists($"user/search?email={email}", HttpContext);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "Email Already Exist.");
            }
            return emailExists;
        }

        //Change Password
        public IActionResult ChangePassword(Guid id)
        {
            ViewBag.id = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(Guid id, ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.id = id;
                return View(model);
            }

            var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}user/change-password/{id}?currentPassword={model.OldPassword}&newPassword={model.NewPassword}");

            var response = await _apiHelper.SendRequestAsync(request, HttpContext);
            if (response.IsSuccessStatusCode)
            {
                TempData["Successful"] = "Password Changed Successfully."; 
                return RedirectToAction("ChangePassword", new { id = id }); 
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var message = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", message);
            }

            ViewBag.id = id;
            return View(model);
        }


    }
}
