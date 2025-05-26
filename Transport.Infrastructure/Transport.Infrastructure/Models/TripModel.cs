namespace Transport.Infrastructure.Models
{
    public class TripModel
    {
        public int Id { get; set; }
        public string DepartureLocation { get; set; } = string.Empty;
        public string DestinationLocation { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        // Зв'язок один-до-багатьох з PassengerModel
        public ICollection<PassengerModel> Passengers { get; set; } = new List<PassengerModel>();
    }
}