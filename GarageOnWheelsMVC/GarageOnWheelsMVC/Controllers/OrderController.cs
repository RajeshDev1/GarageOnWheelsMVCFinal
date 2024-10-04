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

            var userId = SessionHelper.GetUserIdFromToken(HttpContext);
            var garage = await _apiHelper.GetAsync<GarageViewModel>($"garage/by-specificUserId/{userId}", HttpContext);
            if (garage == null || garage.Id == Guid.Empty)
            {
                return BadRequest("Invalid garage data received.");
            }
            var orders = await _apiHelper.GetAsync<List<Order>>($"Order/GetOrdersByGarage/{garage.Id}", HttpContext);
            if (orders == null)
            {
                return View(new List<Order>());
            }
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

                if (response.StatusCode == HttpStatusCode.OK)
                {

                    TempData["Successful"] = "Order successfully created!";

                    if (User.IsInRole("GarageOwner"))
                    {
                        return Json(new { success = true, redirectUrl = Url.Action("GetOrdersByGarage", "Order") });                 
                    }
                    else if (User.IsInRole("Customer"))
                    {
                        return Json(new { success = true, redirectUrl = Url.Action("OrderHistory", "Order") });
                    }
                }
                return View(model);
            }

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

        [Authorize(Roles = "GarageOwner,Customer")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _apiHelper.DeleteAsync($"Order/DeleteOrder/{id}", HttpContext);

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                TempData["Successful"] = "Order successfully deleted!";

                if (User.IsInRole("GarageOwner"))
                {
                    return RedirectToAction("GetOrdersByGarage", "Order");
                }
                else if (User.IsInRole("Customer"))
                {
                    return RedirectToAction("OrderHistory","Order");
                }

            }

            return BadRequest("Error deleting the order.");
        }


[Authorize(Roles = "GarageOwner")]
        public async Task<IActionResult> ViewImages(Guid orderId)
        {
            var viewModel = new UpdateOrderViewModel();

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

 
                // Delete the image file from the server
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }            
               
                var response = await _apiHelper.DeleteAsync($"Order/DeleteOrderImage/{orderId}?fileName={fileName}", HttpContext);
                if(response.StatusCode == HttpStatusCode.NoContent)
                {
                TempData["Successful"] = "Image deleted successfully!";

          /*      return RedirectToAction("OrderDetails", new { id = orderId });*/
                return Json(new { success = true });
            }
            return BadRequest();
                
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



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOrderByCustomer(UpdateOrderViewModel model)
        {
            if (ModelState.IsValid)
            {          
                var UserId = SessionHelper.GetUserIdFromToken(HttpContext);
                var order = new Order
                {
                    Id = model.Order.Id,
                    UserId = model.Order.UserId,
                    GarageId = model.Order.GarageId,
                    ServiceDetails = model.Order.ServiceDetails,
                    OrderDate = DateTime.Now, 
                    TotalAmount = model.Order.TotalAmount,
                    Status = model.Order.Status,
                    CreatedDate = model.Order.CreatedDate,
                    UpdatedDate = DateTime.Now, 
                    CreatedBy = model.Order.CreatedBy,
                    UpdatedBy = model.Order.UpdatedBy,
                    IsDelete = model.Order.IsDelete,
                    ImageUploadByCustomer = model.Order.ImageUploadByCustomer ?? new List<string>() 
                };

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

                var response = await _apiHelper.SendJsonAsync($"order/UpdateOrder/{model.Order.Id}", order, HttpMethod.Put, HttpContext);

                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    TempData["Successful"] = "Order successfully Created.";

                    if (User.IsInRole("GarageOwner"))
                    {
                        return RedirectToAction("GetOrdersByGarage", "Order");
                    }
                    else if (User.IsInRole("Customer"))
                    {
                        return RedirectToAction("OrderHistory", "Order");
                    }

                }
            }

            return View(model);
        }

        public async Task<IActionResult> ViewOrderDetails(Guid id)
        {
            var order = await _apiHelper.GetAsync<OrderViewModel>($"order/GetOrderById/{id}",HttpContext);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }


    }
}
