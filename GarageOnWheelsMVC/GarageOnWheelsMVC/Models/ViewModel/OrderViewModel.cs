namespace GarageOnWheelsMVC.Models.ViewModel
{
    public class OrderViewModel
    {
        public Guid OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
    }
}
