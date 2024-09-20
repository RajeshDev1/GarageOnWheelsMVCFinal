namespace GarageOnWheelsAPI.Models.DatabaseModels
{
    public class Otp
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public int Code { get; set; }
        public DateTime ExpireTime { get; set; }
    }
}
