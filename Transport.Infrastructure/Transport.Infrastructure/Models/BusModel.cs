namespace Transport.Infrastructure.Models
{
    public class BusModel : VehicleModel
    {
        public int Capacity { get; set; }
        public string RouteNumber { get; set; } = string.Empty;
    }
}