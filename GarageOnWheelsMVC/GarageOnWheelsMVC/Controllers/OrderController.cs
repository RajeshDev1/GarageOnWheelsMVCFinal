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

        private readonly HttpClient _httpClient;
        string baseurl = "https://localhost:7107/api/";
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        public OrderController(HttpClient httpClient)
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

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> GetOrdersByGarage()
         {
            SetAuthorize();
            var userId = SessionHelper.GetUserIdFromToken(HttpContext);
            var response = await _httpClient.GetAsync($"{baseurl}Garage/by-specificUserId/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"API Error: {response.ReasonPhrase}");
                return BadRequest("Error fetching garage data from the API.");
            }
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            GarageViewModel garage;
            try
            {
                 garage = JsonSerializer.Deserialize<GarageViewModel>(jsonResponse, options);
            }
            catch (JsonException ex)
            {
                return BadRequest("Error deserializing garage data.");
            }
            if (garage == null || garage.Id == Guid.Empty)
            {
                return BadRequest("Invalid garage data received.");
            }
            response = await _httpClient.GetAsync($"{baseurl}Order/GetOrdersByGarage/{garage.Id}");

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Error fetching orders data from the API.");
            }
            jsonResponse = await response.Content.ReadAsStringAsync();

            List<Order> orders;
            try
            {
                orders = JsonSerializer.Deserialize<List<Order>>(jsonResponse, options);
            }
            catch (JsonException ex)
            {
                return BadRequest("Error deserializing order data.");
            }
            return View(orders);
        }




        [HttpGet]
        [Authorize(Roles ="GarageOwner,Customer")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order model)
        {
            SetAuthorize();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var id = SessionHelper.GetUserIdFromToken(HttpContext);
            model.UserId = id;
            model.CreatedBy = id;
            var jsonmodel = JsonSerializer.Serialize(model);
            var content = new StringContent(jsonmodel, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{baseurl}order/CreateOrder", content);
            if (response.StatusCode == HttpStatusCode.Created)
            {
                return RedirectToAction("Dashboard", "Account");
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var Message = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", Message);
            }
            return View(model);
        }

        [Authorize(Roles = "GarageOwner")]
        public async Task<IActionResult> Edit(Guid id)
        {
            SetAuthorize();
            var response = await _httpClient.GetAsync($"{baseurl}order/GetOrderById/{id}");
            var orders = JsonSerializer.Deserialize<Order>(await response.Content.ReadAsStringAsync(), JsonOptions);
            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Order model)
        {
            SetAuthorize();
            if (!ModelState.IsValid)
            {
                
                return View(model);
            }

            var id = SessionHelper.GetUserIdFromToken(HttpContext);
            model.UpdatedBy = id;

            var jsonModel = JsonSerializer.Serialize(model);
            var content = new StringContent(jsonModel, Encoding.UTF8, "application/json");           
            var response = await _httpClient.PutAsync($"{baseurl}Order/UpdateOrder/{model.Id}", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Dashboard", "Account");
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", string.IsNullOrWhiteSpace(errorMessage) ? "An error occurred while updating the order." : errorMessage);
                return View(model);
            }
        }

        [Authorize(Roles ="Customer")]
        public async Task<IActionResult> OrderHistory(Guid userId)
        {
            SetAuthorize();
            try
            {
                userId = SessionHelper.GetUserIdFromToken(HttpContext);
                var apiUrl = ($"{baseurl}order/GetOrderHistory/{userId}");

                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var orders = await response.Content.ReadFromJsonAsync<List<OrderViewModel>>();

                    if (orders == null || !orders.Any())
                    {
                        ViewBag.Message = "No orders found.";
                        return View(new List<OrderViewModel>());
                    }

                    return View(orders);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    ViewBag.Message = "No orders found.";
                    return View(new List<OrderViewModel>());
                }
                else
                {
                    ViewBag.Message = "An error occurred while fetching orders.";
                    return View(new List<OrderViewModel>());
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"An error occurred: {ex.Message}";
                return View(new List<OrderViewModel>());
            }
        }

        [Authorize(Roles ="GarageOwner")]
        public async Task<IActionResult> Delete(Guid id)
        {
            SetAuthorize();
            string previousUrl = Request.Headers["Referer"].ToString();
            var response = await _httpClient.DeleteAsync($"{baseurl}Order/DeleteOrder/{id}");
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                if (!string.IsNullOrEmpty(previousUrl))
                {
                    return Redirect(previousUrl);
                }
            }
            return BadRequest();
        }
    }
}
