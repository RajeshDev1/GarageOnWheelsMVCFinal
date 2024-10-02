using System.ComponentModel.DataAnnotations;
using static NuGet.Packaging.PackagingConstants;

namespace GarageOnWheelsMVC.Models.ViewModel
{
    public class CreateOrderViewModel
    {
        public Guid Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required(ErrorMessage = "Garage is required")]
        public Guid GarageId { get; set; }
        [Required(ErrorMessage = "Write the required services")]
        public string ServiceDetails { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public Guid CreatedBy { get; set; }

        public List<IFormFile> ImageUploadByCustomer { get; set; }



        public static Order Mapping(CreateOrderViewModel createOrderViewModel)
        {
            return new Order
            {
                Id = createOrderViewModel.Id,
                UserId = createOrderViewModel.UserId,
                GarageId = createOrderViewModel.GarageId,
                ServiceDetails = createOrderViewModel.ServiceDetails,
                OrderDate = createOrderViewModel.OrderDate,
                Status = createOrderViewModel.Status,
                CreatedDate = createOrderViewModel.CreatedDate,
                CreatedBy = createOrderViewModel.CreatedBy,
                ImageUploadByCustomer = new List<string>()
            };
        }

        public static IEnumerable<Order> Mapping(IEnumerable<CreateOrderViewModel> createOrderViewModel)
        {
            var orders = new List<Order>();
            foreach(var order in createOrderViewModel)
            {
                orders.Add(Mapping(order));
            }
            return orders;
        }

        public static CreateOrderViewModel Mapping(Order order)
        {
            return new CreateOrderViewModel
            {
                Id = order.Id,
                UserId = order.UserId,
                GarageId = order.GarageId,
                ServiceDetails = order.ServiceDetails,
                OrderDate = order.OrderDate,
                Status = order.Status,
                CreatedDate = order.CreatedDate,
                CreatedBy = order.CreatedBy,
            };
        }
        public static IEnumerable<CreateOrderViewModel> Mapping(IEnumerable<Order> order)
        {
            var orders = new List<CreateOrderViewModel>();
            foreach (var ord in order)
            {
                orders.Add(Mapping(ord));
            }
            return orders;
        }
    }

}
