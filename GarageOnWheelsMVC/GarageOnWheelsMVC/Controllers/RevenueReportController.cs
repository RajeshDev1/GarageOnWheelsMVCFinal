using GarageOnWheelsMVC.Models; 
using GarageOnWheelsMVC.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using GarageOnWheelsMVC.Helper;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;

namespace GarageOnWheelsMVC.Controllers
{
    public class RevenueReportController : Controller
    {
        private readonly HttpClient _httpClient;
        string baseurl = "https://localhost:7107/api/";
        public RevenueReportController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7107/api/");
        }
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
          
        public void SetAuthorize()
        {
            var token = HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

        }



        [HttpPost]
        [Authorize(Roles ="SuperAdmin,GarageOwner")]
        public async Task<IActionResult> ByDateRange(Guid garageId, DateTime startDate, DateTime endDate)
        {

            SetAuthorize();
            try
            {
                var response = await _httpClient.GetAsync($"RevenueReport/GetRevenueReportByDateRange?garageId={garageId}&startDate={startDate:O}&endDate={endDate:O}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var revenueReports = JsonConvert.DeserializeObject<List<RevenueReportViewModel>>(content);
                return PartialView("_RevenueReportTable",revenueReports);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetGarages()
        {
            SetAuthorize();
            try
            {
                // Call your API to get the list of garages
                var response = await _httpClient.GetAsync($"{baseurl}Garage/all");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var garages = JsonConvert.DeserializeObject<IEnumerable<GarageViewModel>>(content);

                // Return the list of garages as JSON
                return Json(garages.Select(g => new { id = g.Id, name = g.Name }));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Deserialization error: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GenerateRevenue()
        {
            SetAuthorize();
            if (!User.IsInRole("SuperAdmin"))
            {
                var userId = SessionHelper.GetUserIdFromToken(HttpContext);
                var response = await _httpClient.GetAsync($"{baseurl}Garage/by-userid/{userId}");

                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest("Error fetching garage data from the API.");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                List<GarageViewModel> garages;

                garages = JsonConvert.DeserializeObject<List<GarageViewModel>>(jsonResponse);

                if (garages == null || !garages.Any())
                {
                    return BadRequest("Garage data is null.");
                }


                var firstGarage = garages.First();
                ViewBag.GarageId = firstGarage.Id;
            }
            var model = new List<RevenueReportViewModel>();
            return View(model);
        }


    }
}
