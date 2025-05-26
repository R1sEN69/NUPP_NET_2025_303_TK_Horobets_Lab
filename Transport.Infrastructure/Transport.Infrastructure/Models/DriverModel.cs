namespace Transport.Infrastructure.Models
{
    public class DriverModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;

        // Зв'язок багато-до-багатьох з VehicleModel
        public ICollection<VehicleModel> Vehicles { get; set; } = new List<VehicleModel>();
    }
}