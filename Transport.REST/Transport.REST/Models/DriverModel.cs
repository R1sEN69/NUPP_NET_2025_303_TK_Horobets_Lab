namespace Transport.REST.Models
{
    public class DriverModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LicenseNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    public class DriverCreateModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LicenseNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    public class DriverUpdateModel
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? LicenseNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}