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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;



namespace GarageOnWheelsMVC.Controllers
{
    public class UserController : Controller
    {
        private readonly ApiHelper _apiHelper;
        private readonly string baseUrl;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public UserController(ApiHelper apiHelper, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _apiHelper = apiHelper;
            baseUrl = configuration["AppSettings:BaseUrl"];
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Dashboard()
        {
            return View();
        }


        //Get All User
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _apiHelper.GetAsync<List<User>>("user/all", HttpContext);
            if (users == null)
            {
                return View(new List<User>());  
            }
            return View(users);
        }

        //Get All Customer
        [Authorize(Roles = "GarageOwner")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var garageOwnerId = SessionHelper.GetUserIdFromToken(HttpContext);
          
            var users = await _apiHelper.GetAsync<List<User>>($"user/allCustomer?garageOwnerId={garageOwnerId}", HttpContext);

            if (users == null)
            {
                return View(new List<User>());
               
            }

            return View(users);

        }


        //Ge all GarageOwner
        [Authorize(Roles = "SuperAdmin,GarageOwner")]
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



        [Authorize(Roles = "SuperAdmin,GarageOwner")]
        public IActionResult Create()
        {
            var model = new RegisterViewModel
            {
                Gender = Gender.Male
            };
            return View(model);
        }

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
                ModelState.AddModelError("", "Email already exists.");
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

            // Send the create user request
            var response = await _apiHelper.SendPostRequest("user/create", userModel, HttpContext);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                await _apiHelper.SendOtp(model.Email, HttpContext);
                TempData["Email"] = model.Email;
                return RedirectToAction("VerifyOtp", "User");
            }
            ModelState.AddModelError("", "An error occurred while creating the user.");
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
            var response = await _apiHelper.SendJsonAsync(request.RequestUri.ToString(), model, HttpMethod.Post, HttpContext);

            if (response.IsSuccessStatusCode)
            {
                TempData["Successful"] = "OTP Verified Successfully!";
                string redirectUrl = Url.Action("GetAllUsers", "User"); 

                if (User.IsInRole("GarageOwner"))
                {
                    redirectUrl = Url.Action("GetAllCustomers", "User");
                }

                // Return JSON response with redirect URL
                return Json(new { success = true, redirectUrl });
            }

            ModelState.AddModelError("", "Invalid OTP. Please try again.");
            TempData.Keep("Email");
            return Json(new { success = false, errorMessage = "Invalid OTP. Please try again." });
        }




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
        public async Task<IActionResult> Edit(UpdateUserViewModel model, IFormFile? UpdateImage)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.UpdatedBy = SessionHelper.GetUserIdFromToken(HttpContext);

            var userModel = UpdateUserViewModel.mapping(model);

            if (Request.Form["DeleteImage"] == "true")
            {

                userModel.ProfileImage = null; 

                if (!string.IsNullOrEmpty(model.ProfileImage))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", model.ProfileImage);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
            }
            else
            {
                // Handle file upload/update
                if (UpdateImage != null && UpdateImage.Length > 0)
                {
                    if (model.ProfileImage == null)
                    {
                        userModel.ProfileImage = await SaveFileAsync(UpdateImage);
                    }
                    else
                    {
                        userModel.ProfileImage = await SaveUpdatedFileAsync(model.ProfileImage, UpdateImage);
                    }
                }
            }

            // API call to update user
            var response = await _apiHelper.SendJsonAsync($"user/update/{model.Id}", userModel, HttpMethod.Put, HttpContext);
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                TempData["Successful"] = "User Updated Successfully.";
                if (User.IsInRole("SuperAdmin"))
                {
                    return RedirectToAction("GetAllUsers");
                }
                else if (User.IsInRole("GarageOwner"))
                {
                    return RedirectToAction("GetAllCustomers");
                }
            }

            // If update fails, show error
            ModelState.AddModelError(string.Empty, "Failed to update user. Please try again.");
            return View(model);
        }



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
        public async Task<IActionResult> EditProfile(UpdateUserViewModel model, IFormFile? UpdateImage)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.UpdatedBy = SessionHelper.GetUserIdFromToken(HttpContext);

            // Check if the image should be deleted
            if (Request.Form["DeleteImage"] == "true")
            {
                // Remove the existing profile image file from the uploads folder
                if (!string.IsNullOrEmpty(model.ProfileImage))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", model.ProfileImage);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath); // Delete the file from server
                    }
                }

                // Set the ProfileImage to a default value
                model.ProfileImage = "default-placeholder.png"; // Set to your default image file name
            }
            else
            {
                // Update the profile image if provided
                if (UpdateImage != null && UpdateImage.Length > 0)
                {
                    if (model.ProfileImage == null || model.ProfileImage == "default-placeholder.png")
                    {
                        model.ProfileImage = await SaveFileAsync(UpdateImage);
                    }
                    else
                    {
                        model.ProfileImage = await SaveUpdatedFileAsync(model.ProfileImage, UpdateImage);
                    }
                }
            }

            // Send the updated data to the API
            var response = await _apiHelper.SendJsonAsync($"user/update/{model.Id}", model, HttpMethod.Put, HttpContext);

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                TempData["Successful"] = "Profile Updated Successfully";

                // Update the user’s claims after successful profile update
                var userId = SessionHelper.GetUserIdFromToken(HttpContext); // Assuming this gets the current user ID
                var role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
                var name = $"{model.FirstName} {model.LastName}".Trim();
                var profileImg = model.ProfileImage;

                // Call the method to update the user’s claims
                await UpdateSignInUser(role, userId, name, profileImg);

                return RedirectToAction("EditProfile", "User");
            }

            ModelState.AddModelError(string.Empty, "Failed to update profile. Please try again.");
            return View(model);
        }





        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(Guid id)
        {

            var orders = await _apiHelper.GetAsync<List<Order>>($"order/by-userid/{id}", HttpContext);

            // Check for any pending orders
            var pendingOrders = orders.Where(o => o.Status == OrderStatus.Pending).ToList();

            if (pendingOrders.Count > 0)
            {
                TempData["Warning"] = "This user has pending orders and cannot be deleted.";
                return RedirectToAction("GetAllUsers");
            }

  
            return RedirectToAction("DeleteConfirmed", new { id = id });
        }

        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var endpoint = $"user/delete/{id}";
            var response = await _apiHelper.DeleteAsync(endpoint, HttpContext);

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                TempData["Successful"] = "User Deleted Successfully.";
                return RedirectToAction("GetAllUsers");
            }

            TempData["Error"] = $"An error occurred while deleting the user. Status Code: {response.StatusCode}";
            return RedirectToAction("GetAllUsers");
        }

        // check the email exist or not
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
        private async Task UpdateSignInUser(string role, Guid id, string name, string img)
        {
            var existingClaims = HttpContext.User.Claims.ToList();

            // Remove any existing claims that you want to replace
            existingClaims.RemoveAll(c => c.Type == ClaimTypes.Role);
            existingClaims.RemoveAll(c => c.Type == ClaimTypes.NameIdentifier);
            existingClaims.RemoveAll(c => c.Type == ClaimTypes.Name);
            existingClaims.RemoveAll(c => c.Type == "profileImg");

            // Add new claims
            existingClaims.Add(new Claim(ClaimTypes.Role, role));
            existingClaims.Add(new Claim(ClaimTypes.NameIdentifier, id.ToString()));
            existingClaims.Add(new Claim(ClaimTypes.Name, name));
            existingClaims.Add(new Claim("profileImg", img != null ? img : string.Empty));

            var identity = new ClaimsIdentity(existingClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Sign the user in again with updated claims
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

    }
}