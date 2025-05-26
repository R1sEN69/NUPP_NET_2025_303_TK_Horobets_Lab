namespace Transport.Infrastructure.Models
{
    public class CarModel : VehicleModel
    {
        public int NumberOfDoors { get; set; }
        public int Year { get; set; }
        public string FuelType { get; set; } = string.Empty;
    }
}
