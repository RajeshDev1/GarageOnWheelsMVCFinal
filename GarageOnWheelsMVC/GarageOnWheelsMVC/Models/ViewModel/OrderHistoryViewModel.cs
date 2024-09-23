namespace GarageOnWheelsMVC.Models.ViewModel
{
    public class OrderHistoryViewModel
    {
        public List<OrderViewModel> Orders { get; set; }    
        public PagingInfo PagingInfo { get; set; }
    }
}
