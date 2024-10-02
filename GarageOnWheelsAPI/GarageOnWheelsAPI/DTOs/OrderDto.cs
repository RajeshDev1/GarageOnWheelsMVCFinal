using GarageOnWheelsAPI.Enums;
using GarageOnWheelsAPI.Models.DatabaseModels;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace GarageOnWheelsAPI.DTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid GarageId { get; set; }
        [Required]
        public string ServiceDetails { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
      
        public decimal TotalAmount { get; set; }
        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public DateTime UpdatedDate { get; set; }
        [Required]
        public Guid CreatedBy { get; set; }
        [Required]
        public Guid UpdatedBy { get; set; }
        [Required]
        public bool IsDelete { get; set; } = false;
        public IList<string> ImageUploadByCustomer { get; set; } = new List<string>();



        public static Orders Mapping(OrderDto orderDto)
        {
            return new Orders
            {
                Id = orderDto.Id,
                UserId = orderDto.UserId,
                GarageId = orderDto.GarageId,
                ServiceDetails = orderDto.ServiceDetails,
                OrderDate = orderDto.OrderDate,
                TotalAmount = orderDto.TotalAmount,
                Status = (int)orderDto.Status,
                CreatedDate = orderDto.CreatedDate,
                UpdatedDate = orderDto.UpdatedDate,
            

            };
        }


        public static IEnumerable<Orders> Mapping(IEnumerable<OrderDto> orderDto)
        { 
            var orders = new List<Orders>();
            foreach (var order in orderDto) 
            { 
                orders.Add(Mapping(order));
            }
            return orders;
        }

        public static OrderDto Mapping(Orders order)
        {
            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                GarageId = order.GarageId,
                ServiceDetails = order.ServiceDetails,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = (OrderStatus)order.Status,
                CreatedDate = order.CreatedDate,
                UpdatedDate = order.UpdatedDate,
                CreatedBy = order.CreatedBy,
                UpdatedBy = order.UpdatedBy,
                IsDelete = order.IsDelete,
                
            };
        }


        public static IEnumerable<OrderDto> Mapping(IEnumerable<Orders> order)
        {
            var orders = new List<OrderDto>();
            foreach (var ord in order)
            {
                orders.Add(Mapping(ord));
            }
            return orders;
        }

    }
}
