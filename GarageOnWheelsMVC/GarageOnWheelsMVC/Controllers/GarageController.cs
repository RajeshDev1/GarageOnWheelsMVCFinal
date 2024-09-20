using GarageOnWheelsMVC.Helper;
using GarageOnWheelsMVC.Models;
using GarageOnWheelsMVC.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using NuGet.Common;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Text.Json;

namespace GarageOnWheelsMVC.Controllers
{
   
    public class GarageController : Controller
    {

        private readonly HttpClient _httpClient;
        string baseurl = "https://localhost:7107/api/";
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        public GarageController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void SetAuthorize()
        {
            var token = HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

        }

        public IActionResult Dashboard()
        {
            return View();  
        }

        [Authorize]
        public async Task<IActionResult> GetAllGarages()
        {
            SetAuthorize();
            var response = await _httpClient.GetAsync($"{baseurl}Garage/all");
            List<GarageViewModel> garages;
            try
            {
                garages = JsonSerializer.Deserialize<List<GarageViewModel>>(await response.Content.ReadAsStringAsync(), JsonOptions);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Deserialization error: {ex.Message}");
                return BadRequest("Error deserializing user data.");
            }
            return View(garages);
        }


        // get all garage in order controller
        [Authorize]
        public async Task<IActionResult> GetAllGarageNames()
        {
            SetAuthorize();
            var response = await _httpClient.GetAsync($"{baseurl}Garage/all");
            List<GarageViewModel> garages;
            try
            {
                garages = JsonSerializer.Deserialize<List<GarageViewModel>>(await response.Content.ReadAsStringAsync(), JsonOptions);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Deserialization error: {ex.Message}");
                return BadRequest("Error deserializing user data.");
            }
            return new JsonResult(garages);
        }

        public async Task<IActionResult> GetGaragesByUserId()   
        {
            SetAuthorize();
            var id = SessionHelper.GetUserIdFromToken(HttpContext);
            var response = await _httpClient.GetAsync($"{baseurl}garage/by-userid/{id}");
            List<GarageViewModel> garages;
            try
            {
                garages = JsonSerializer.Deserialize<List<GarageViewModel>>(await response.Content.ReadAsStringAsync(), JsonOptions);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Deserialization error: {ex.Message}");
                return BadRequest("Error deserializing user data.");
            }
            return View(garages);
        }

        public async Task<IActionResult> GetGaragesBySpecificUserId()
        {
            SetAuthorize();
            var id = SessionHelper.GetUserIdFromToken(HttpContext);
            var response = await _httpClient.GetAsync($"{baseurl}garage/by-userid/{id}");
            List<GarageViewModel> garages;
            try
            {
                garages = JsonSerializer.Deserialize<List<GarageViewModel>>(await response.Content.ReadAsStringAsync(), JsonOptions);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Deserialization error: {ex.Message}");
                return BadRequest("Error deserializing user data.");
            }
            return new JsonResult(garages);
        }

        // Here u can Perform 2 Operation(Create a Garage, Update a Garage)

        [Authorize(Roles ="SuperAdmin,GarageOwner")]
        public async Task<IActionResult> Delete(Guid id)
        {
            SetAuthorize();
            var response = await _httpClient.DeleteAsync($"{baseurl}garage/delete/{id}");
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return RedirectToAction("GetAllGarages", "Garage");
            }
            return BadRequest();
        }


        [HttpGet]

        [Authorize(Roles = "SuperAdmin,GarageOwner")]
        public async Task<IActionResult> Save(Guid? id)
        {
            SetAuthorize();

            GarageViewModel model = new GarageViewModel();

            if (id.HasValue)
            {
                var response = await _httpClient.GetAsync($"{baseurl}garage/{id.Value}");
                if (response.IsSuccessStatusCode)
                {
                    model = JsonSerializer.Deserialize<GarageViewModel>(await response.Content.ReadAsStringAsync(), JsonOptions);
                }
            }
            return View("Save", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(GarageViewModel model)
        {
            SetAuthorize();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.UserId == Guid.Empty)
            {
                model.UserId = SessionHelper.GetUserIdFromToken(HttpContext);
            }

        
            if (model.Id == Guid.Empty) 
            {
                
                var existingGarageResponse = await _httpClient.GetAsync($"{baseurl}garage/getbyownerid?ownerId={model.UserId}");
                if (existingGarageResponse.IsSuccessStatusCode)
                {
                    var existingGarage = JsonSerializer.Deserialize<bool>(await existingGarageResponse.Content.ReadAsStringAsync(), JsonOptions);
                    if (existingGarage)
                    {
                        ModelState.AddModelError("", "You already have a garage. Only one garage can be created per owner.");
                        return View(model);
                    }
                }

                model.CreatedBy = SessionHelper.GetUserIdFromToken(HttpContext);

                
                var jsonModel = JsonSerializer.Serialize(model);
                var content = new StringContent(jsonModel, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{baseurl}garage/create", content);

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    return RedirectToAction("GetAllGarages", "Garage");
                }
            }
            else 
            {
                model.UpdatedBy = SessionHelper.GetUserIdFromToken(HttpContext);

                // Serialize and Put (update) the garage data
                var jsonModel = JsonSerializer.Serialize(model);
                var content = new StringContent(jsonModel, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{baseurl}garage/update/{model.Id}", content);

                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return RedirectToAction("GetAllGarages", "Garage");
                }
            }

            return View(model);
        }


    }
}
