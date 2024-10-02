namespace GarageOnWheelsAPI.Models.DatabaseModels
{
    public class OrderFiles
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string? FileName { get; set; }
        public DateTime UploadDate { get; set; }

        public virtual Orders Orders { get; set; }

    }
}
