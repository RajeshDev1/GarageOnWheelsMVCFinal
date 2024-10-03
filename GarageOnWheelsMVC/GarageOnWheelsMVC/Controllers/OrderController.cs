using GarageOnWheelsMVC.Helper;
using GarageOnWheelsMVC.Models;
using GarageOnWheelsMVC.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.MSIdentity.Shared;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace GarageOnWheelsMVC.Controllers
{
    public class OrderController : Controller
    {

        private readonly ApiHelper _apiHelper;
        private readonly IHttpClientFactory _httpClientFactory;
        public OrderController(ApiHelper apiHelper, IHttpClientFactory httpClientFactory)
        {
            _apiHelper = apiHelper;
            _httpClientFactory = httpClientFactory;
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
            var orders = await _apiHelper.GetAsync<List<Order>>($"Order/GetOrdersByGarage/{garage.Id}", HttpContext);

            if (orders == null)
            {
                return View(new List<Order>());
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
        public async Task<IActionResult> Create(CreateOrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                var order = CreateOrderViewModel.Mapping(model);

                var userId = SessionHelper.GetUserIdFromToken(HttpContext);
                order.UserId = userId;
                order.CreatedBy = userId;
                order.CreatedDate = DateTime.Now;
                order.OrderDate = DateTime.Now;
           

                if (model.ImageUploadByCustomer != null && model.ImageUploadByCustomer.Count > 0)
                {
                    var imageFileNames = new List<string>();

                    foreach (var file in model.ImageUploadByCustomer)
                    {
                        if (file.Length > 0)
                        {

                            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                            var filePath = Path.Combine(uploadFolder, uniqueFileName);

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }

                            imageFileNames.Add(uniqueFileName);
                        }
                    }

                    order.ImageUploadByCustomer.AddRange(imageFileNames);
                }

                var response = await _apiHelper.SendPostRequest("order/CreateOrder", order, HttpContext);

                return Json(new { redirectUrl = Url.Action("OrderHistory", "Order") });
            }

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


/*        public async Task<IActionResult> Delete(Guid id)
        {

            var order = await _apiHelper.GetAsync<List<Assignment>>($"{_baseUrl}assignment/by-siteid/{id}", HttpContext);

            var inspectionfinding = await _httphelper.GetAsync<List<InspectionFinding>>($"{_baseUrl}inspectionfinding/by-site/{id}", HttpContext);

            var existinspectionFinding = inspectionfinding.Where(i => i.ResolutionStatus != ResolutionStatus.Resolved).ToList();

            if (assignment.Count != 0 || existinspectionFinding.Count != 0)

            {

                TempData["Warning"] = "This site has assignments and cannot be deleted.";

                return RedirectToAction("GetOrdersByGarage");

            }



            return RedirectToAction("DeleteConfirmed", new { id = id });
        }

*/


        [Authorize(Roles = "GarageOwner")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var response = await _apiHelper.DeleteAsync($"Order/DeleteOrder/{id}", HttpContext);

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                TempData["Successful"] = "Garage successfully deleted!";
                return RedirectToAction("GetOrdersByGarage", "Order");
            }

            return BadRequest("Error deleting the order.");
        }



        [Authorize(Roles = "GarageOwner")]
        public async Task<IActionResult> ViewImages(Guid orderId)
        {
            var viewModel = new UpdateOrderViewModel();
           /* viewModel.OrderId = orderId;*/

            // Call API to fetch OrderFiles
            var apiResponse = await _apiHelper.GetAsync<List<OrderFilesDto>>($"order/GetOrderImages/{orderId}", HttpContext);

            if (apiResponse != null)
            {
                viewModel.OrderFiles = apiResponse;
            }

            return View(viewModel);
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> DeleteOrderImage(Guid orderId, string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                TempData["Error"] = "File name is required.";
                return RedirectToAction("OrderDetails", new { id = orderId });
            }

            // Consuming the API to delete the image by OrderId and FileName
            var response = await _apiHelper.DeleteAsync($"Order/DeleteOrderImage/{orderId}?fileName={fileName}", HttpContext);

            if (response.IsSuccessStatusCode)
            {
                TempData["Successful"] = "Image deleted successfully!";
                return RedirectToAction("OrderDetails", new { id = orderId });
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Error deleting the image: {errorMessage}";
                return RedirectToAction("OrderDetails", new { id = orderId });
            }
        }

        public async Task<IActionResult> EditOrderByCustomer(Guid id)
        {
            var order = await _apiHelper.GetAsync<Order>($"order/GetOrderById/{id}", HttpContext);
            var orderFiles = await _apiHelper.GetAsync<List<OrderFilesDto>>($"order/GetOrderImages/{id}", HttpContext);

            var model = new UpdateOrderViewModel()
            {
                Order = order,
                OrderFiles = orderFiles,
            };
            return View(model);
        }
    }
}
