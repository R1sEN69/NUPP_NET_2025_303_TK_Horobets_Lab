namespace Transport.Infrastructure.Models
{
    public class EngineModel
    {
        public int Id { get; set; }
        public int Horsepower { get; set; }
        public int Cylinders { get; set; }

        // Навігаційна властивість для зворотного зв'язку
        public VehicleModel? Vehicle { get; set; }
    }
}