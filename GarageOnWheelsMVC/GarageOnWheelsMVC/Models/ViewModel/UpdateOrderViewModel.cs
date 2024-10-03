namespace GarageOnWheelsMVC.Models.ViewModel
{
    public class UpdateOrderViewModel
    {
        public Order Order { get; set; }
        public List<OrderFilesDto>? OrderFiles { get; set; }
        public List<IFormFile>? ImageUploadByCustomer { get; set; }


    }
}











