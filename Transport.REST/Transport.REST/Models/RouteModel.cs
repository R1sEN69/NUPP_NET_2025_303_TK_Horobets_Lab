namespace Transport.REST.Models
{
    public class RouteModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } 
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
        public decimal DistanceKm { get; set; } 
    }

    public class RouteCreateModel
    {
        public string Name { get; set; }
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
        public decimal DistanceKm { get; set; }
    }

    public class RouteUpdateModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? StartPoint { get; set; }
        public string? EndPoint { get; set; }
        public decimal? DistanceKm { get; set; }
    }
}