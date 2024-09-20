namespace GarageOnWheelsAPI.Models.DatabaseModels
{
    public class RegisterResult
    {
        public bool Success { get; set; }
        public User? User { get; set; }
        public IList<string> Errors { get; set; } = new List<string>();
    }
}
