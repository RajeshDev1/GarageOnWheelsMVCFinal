namespace GarageOnWheelsMVC.Models.ViewModel
{
    public class ViewImagesViewModel
    {
        public Guid OrderId { get; set; }
        public List<OrderFilesDto> OrderFiles { get; set; }
    }
}
