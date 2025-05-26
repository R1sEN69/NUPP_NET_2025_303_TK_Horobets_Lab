namespace Transport.Infrastructure.Models
{
    public class PassengerModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string TicketNumber { get; set; } = string.Empty;

        public int TripId { get; set; }
        public TripModel? Trip { get; set; }
    }
}