namespace Transport.Infrastructure.Models
{
    public abstract class VehicleModel
    {
        public int Id { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int MaxSpeed { get; set; }
        public int YearManufactured { get; set; }

        // Зв'язок один-до-одного з EngineModel
        public int? EngineId { get; set; } // Foreign Key
        public EngineModel? Engine { get; set; }

        // Зв'язок багато-до-багатьох з DriverModel
        public ICollection<DriverModel> Drivers { get; set; } = new List<DriverModel>();
    }
}