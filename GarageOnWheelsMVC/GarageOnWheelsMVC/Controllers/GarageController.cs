using GarageOnWheelsMVC.Helper;
using GarageOnWheelsMVC.Models;
using GarageOnWheelsMVC.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using NuGet.Common;
using System.Drawing.Printing;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Text.Json;

namespace GarageOnWheelsMVC.Controllers
{
   
    public class GarageController : Controller
    {
        private readonly ApiHelper _apiHelper;

        public GarageController(ApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public IActionResult Dashboard()
        {
            return View();  
        }

        [Authorize]
        public async Task<IActionResult> GetAllGarages()
        {
            var garages = await _apiHelper.GetAsync<List<GarageViewModel>>("Garage/all",HttpContext);
                
            if (garages == null)
            {
                return BadRequest("Error retrieving garages.");
            }     
            return View(garages);
        }


        // get all garage   
        [Authorize]
        public async Task<IActionResult> GetAllGarageNames()
        {
            var garages = await _apiHelper.GetAsync<List<GarageViewModel>>("Garage/all", HttpContext);
            if (garages == null)
            {
                return BadRequest("Error retrieving garage names.");
            }
            return new JsonResult(garages);
        }

        public async Task<IActionResult> GetGaragesByUserId()
        {
            var id = SessionHelper.GetUserIdFromToken(HttpContext);
            var garages = await _apiHelper.GetAsync<List<GarageViewModel>>($"garage/by-userid/{id}", HttpContext);
            if (garages == null)
            {
                return BadRequest("Error retrieving garages.");
            }           
            return View(garages);
           
        }
        public async Task<IActionResult> GetGaragesBySpecificUserId()
        {
            var id = SessionHelper.GetUserIdFromToken(HttpContext);
            var garages = await _apiHelper.GetAsync<List<GarageViewModel>>($"garage/by-userid/{id}", HttpContext);
            if (garages == null)
            {
                return BadRequest("Error retrieving garages.");
            }
            return new JsonResult(garages);
        }



        [Authorize(Roles = "SuperAdmin,GarageOwner")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _apiHelper.DeleteAsync($"garage/delete/{id}", HttpContext);
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
            GarageViewModel model = new GarageViewModel();

            if (id.HasValue)
            {
                model = await _apiHelper.GetAsync<GarageViewModel>($"garage/{id.Value}", HttpContext);
                if (model == null)
                {
                    return BadRequest("Error retrieving garage data.");
                }
                model.UserId = model.UserId; 
            }
            else
            {
                model.UserId = Guid.Empty; 
            }

            return View("Save", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(GarageViewModel model)
        {
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
                var existingGarage = await _apiHelper.GetBoolAsync($"garage/getbyownerid?ownerId={model.UserId}", HttpContext);
                if (existingGarage)
                {
                    ModelState.AddModelError("", "You already have a garage. Only one garage can be created per owner.");
                    return View(model);
                }

                model.CreatedBy = SessionHelper.GetUserIdFromToken(HttpContext);
                var response = await _apiHelper.SendPostRequest("garage/create", model, HttpContext);

                if (response.StatusCode == HttpStatusCode.Created)
                {                  
                    TempData["Successful"] = "Garage Successfully Created.";
                    return User.IsInRole("SuperAdmin")
                        ? RedirectToAction("GetAllGarages", "Garage")
                        : RedirectToAction("GetGaragesByUserId", "Garage");
                }
            }
            else
            {
                model.UpdatedBy = SessionHelper.GetUserIdFromToken(HttpContext);
                var response = await _apiHelper.SendJsonAsync($"garage/update/{model.Id}", model, HttpMethod.Put, HttpContext);

                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    // Store a success message in TempData for the next request
                    TempData["Successful"] = "Garage successfully Updated.";
                    return User.IsInRole("SuperAdmin")
                        ? RedirectToAction("GetAllGarages", "Garage")
                        : RedirectToAction("GetGaragesByUserId", "Garage");
                }
            }

            return View(model);
        }



    }
}
