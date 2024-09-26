using GarageOnWheelsMVC.Helper;
using GarageOnWheelsMVC.Models;
using GarageOnWheelsMVC.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarageOnWheelsMVC.Controllers
{
    public class RevenueReportController : Controller
    {
        private readonly ApiHelper _apiHelper;

        public RevenueReportController(ApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,GarageOwner")]
        public async Task<IActionResult> ByDateRange(Guid garageId, DateTime startDate, DateTime endDate)
        {
            var errors = new Dictionary<string, string>();

            // Validate the inputs
            if (garageId == Guid.Empty)
            {
                errors.Add("GarageId", "Please select a valid garage.");
            }

            if (startDate == default)
            {
                errors.Add("StartDate", "Start date is required.");
            }

            if (endDate == default)
            {
                errors.Add("EndDate", "End date is required.");
            }

            if (startDate != default && endDate != default && startDate > endDate)
            {
                errors.Add("EndDate", "End date cannot be earlier than start date.");
            }

            // If there are validation errors, return them
            if (errors.Count > 0)
            {
                return Json(new { success = false, errors });
            }

            try
            {
                // Use ApiHelper to fetch the revenue reports by date range
                var endpoint = $"RevenueReport/GetRevenueReportByDateRange?garageId={garageId}&startDate={startDate:O}&endDate={endDate:O}";
                var revenueReports = await _apiHelper.GetAsync<List<RevenueReportViewModel>>(endpoint, HttpContext);

                if (revenueReports != null)
                {
                    return PartialView("_RevenueReportTable", revenueReports);
                }
                else
                {
                    return Json(new { success = false, errors = new { General = "Error fetching revenue reports." } });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = new { General = $"Error: {ex.Message}" } });
            }
        }



        [HttpGet]
        public async Task<IActionResult> GetGarages()
        {
            try
            {
                // Use ApiHelper to get the list of garages
                var garages = await _apiHelper.GetAsync<IEnumerable<GarageViewModel>>("Garage/all", HttpContext);

                if (garages != null)
                {
                    // Return the list of garages as JSON
                    return Json(garages.Select(g => new { id = g.Id, name = g.Name }));
                }
                else
                {
                    return BadRequest("Error fetching garages.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GenerateRevenue()
        {
            if (!User.IsInRole("SuperAdmin"))
            {
                var userId = SessionHelper.GetUserIdFromToken(HttpContext);

                var endpoint = $"Garage/by-userid/{userId}";
                var garages = await _apiHelper.GetAsync<List<GarageViewModel>>(endpoint, HttpContext);

                if (garages == null || !garages.Any())
                {
                    return BadRequest("Garage data is null.");
                }

                var firstGarage = garages.First();
                ViewBag.GarageId = firstGarage.Id;
            }

            // Return an empty model
            var model = new List<RevenueReportViewModel>();
            return View(model);
        }
    }
}