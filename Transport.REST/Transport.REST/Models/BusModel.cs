namespace Transport.REST.Models
{
    public class BusModel
    {
        public Guid Id { get; set; }
        public string PlateNumber { get; set; } 
        public string Model { get; set; } 
        public int Capacity { get; set; }
        public Guid? DriverId { get; set; }
        public Guid? RouteId { get; set; }
    }

    public class BusCreateModel
    {
        public string PlateNumber { get; set; }
        public string Model { get; set; }
        public int Capacity { get; set; }
        public Guid? DriverId { get; set; }
        public Guid? RouteId { get; set; }
    }

    public class BusUpdateModel
    {
        public Guid Id { get; set; } 
        public string? PlateNumber { get; set; }
        public string? Model { get; set; }
        public int? Capacity { get; set; }
        public Guid? DriverId { get; set; }
        public Guid? RouteId { get; set; }
    }
}