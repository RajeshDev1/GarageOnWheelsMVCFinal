namespace GarageOnWheelsMVC.Models.ViewModel
{
    public class GarageListViewModel
    {
        public List<GarageViewModel> Garages { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
