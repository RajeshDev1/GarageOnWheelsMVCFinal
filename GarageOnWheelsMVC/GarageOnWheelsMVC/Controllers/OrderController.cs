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
        public async Task<IActionResult> GetOrdersByGarage(int page = 1, int pageSize = 3)
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

            // Total count of orders
            var totalCount = orders.Count;

            // Paginate the orders
            var paginatedOrders = orders.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Create the view model with paginated orders and paging info
            var viewModel = new OrderListViewModel
            {
                Orders = paginatedOrders,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    TotalItems = totalCount,
                    ItemsPerPage = pageSize
                }
            };

            // Return the paginated orders to the view
            return View(viewModel);
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
                return View(model);
            }

            var userId = SessionHelper.GetUserIdFromToken(HttpContext);
            model.UserId = userId;
            model.CreatedBy = userId;

            var response = await _apiHelper.SendPostRequest("order/CreateOrder", model, HttpContext);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                return RedirectToAction("Dashboard", "Account");
            }

            var message = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", message);

            return View(model);
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

            var userId = SessionHelper.GetUserIdFromToken(HttpContext);
            model.UpdatedBy = userId;

            var response = await _apiHelper.SendJsonAsync($"Order/UpdateOrder/{model.Id}", model, HttpMethod.Put, HttpContext);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Dashboard", "Account");
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", string.IsNullOrWhiteSpace(errorMessage) ? "An error occurred while updating the order." : errorMessage);

            return View(model);
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> OrderHistory(int page = 1, int pageSize = 5)
        {
            var userId = SessionHelper.GetUserIdFromToken(HttpContext);
            var orders = await _apiHelper.GetAsync<List<OrderViewModel>>($"order/GetOrderHistory/{userId}", HttpContext);

            if (orders == null || !orders.Any())
            {
                ViewBag.Message = "No orders found.";
                return View(new List<OrderViewModel>());
            }
            var totalCount = orders.Count;
            var paginatedOrders = orders.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var viewModel = new OrderHistoryViewModel
            {
                Orders = paginatedOrders,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    TotalItems = totalCount,
                    ItemsPerPage = pageSize
                }
            };

            return View(viewModel);
        }


        [Authorize(Roles = "GarageOwner")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _apiHelper.DeleteAsync($"Order/DeleteOrder/{id}", HttpContext);

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                string previousUrl = Request.Headers["Referer"].ToString();
                if (!string.IsNullOrEmpty(previousUrl))
                {
                    return Redirect(previousUrl);
                }
            }

            return BadRequest("Error deleting the order.");
        }

    }
}
