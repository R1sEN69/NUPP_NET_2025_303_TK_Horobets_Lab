namespace Transport.Infrastructure.Models
{
    public class TrainModel : VehicleModel
    {
        public int NumberOfWagons { get; set; }
        public string LocomotiveType { get; set; } = string.Empty;
    }
}