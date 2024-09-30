using GarageOnWheelsMVC.Helper;
using GarageOnWheelsMVC.Models;
using GarageOnWheelsMVC.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.MSIdentity.Shared;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace GarageOnWheelsMVC.Controllers
{
    public class OrderController : Controller
    {

        private readonly ApiHelper _apiHelper;

        public OrderController(ApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

    
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> GetOrdersByGarage()
        {
            // Fetch the user ID from the session
            var userId = SessionHelper.GetUserIdFromToken(HttpContext); 

            // Get the garage associated with the user
            var garage = await _apiHelper.GetAsync<GarageViewModel>($"garage/by-specificUserId/{userId}", HttpContext);

            if (garage == null || garage.Id == Guid.Empty)
            {
                return BadRequest("Invalid garage data received.");
            }

            // Fetch the orders for the garage
            var orders = await _apiHelper.GetAsync<List<Order>>($"Order/GetOrdersByGarage/{garage.Id}",HttpContext);

            if (orders == null)
            {
                return BadRequest("Error fetching orders data from the API.");
            }
          
            // Return the paginated orders to the view
            return View(orders);
        }

        [HttpGet]
        [Authorize(Roles = "GarageOwner,Customer")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Returns the same view with validation messages
            }

            var userId = SessionHelper.GetUserIdFromToken(HttpContext);
            model.UserId = userId;
            model.CreatedBy = userId;

            var response = await _apiHelper.SendPostRequest("order/CreateOrder", model, HttpContext);

                
                // Redirect based on user role
                TempData["Successful"] = "Order Created Successfully.";
                if (User.IsInRole("GarageOwner"))
                {
                    return Json(new { success = true, redirectUrl = Url.Action("GetOrdersByGarage", "Order") });
                  
                }
                else if (User.IsInRole("Customer"))
                {
                    return Json(new { success = true, redirectUrl = Url.Action("OrderHistory", "Order") });
                  
                }
            

            var message = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", message);

            return View(model); // Return the same view with error message
        }


        [Authorize(Roles = "GarageOwner")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var order = await _apiHelper.GetAsync<Order>($"order/GetOrderById/{id}", HttpContext);

            if (order == null)
            {
                return BadRequest("Order not found.");
            }

            return View(order);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Order model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }        
            model.Status = OrderStatus.Completed;

            var userId = SessionHelper.GetUserIdFromToken(HttpContext);
            model.UpdatedBy = userId;

            var response = await _apiHelper.SendJsonAsync($"Order/UpdateOrder/{model.Id}", model, HttpMethod.Put, HttpContext);

            if (response.IsSuccessStatusCode)
            {
                TempData["Successful"] = "Order Updated Successfully.";
                return RedirectToAction("GetOrdersByGarage", "Order");
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", string.IsNullOrWhiteSpace(errorMessage) ? "An error occurred while updating the order." : errorMessage);

            return View(model);
        }



        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> OrderHistory()
        {
            var userId = SessionHelper.GetUserIdFromToken(HttpContext);
            var orders = await _apiHelper.GetAsync<List<OrderViewModel>>($"order/GetOrderHistory/{userId}", HttpContext);

            if (orders == null || !orders.Any())
            {
                ViewBag.Message = "No orders found.";
                return View(new List<OrderViewModel>());
            }

            return View(orders);
        }

        [Authorize(Roles = "GarageOwner")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _apiHelper.DeleteAsync($"Order/DeleteOrder/{id}", HttpContext);

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return RedirectToAction("GetOrdersByGarage", "Order");
            }

            return BadRequest("Error deleting the order.");
        }

    }
}
