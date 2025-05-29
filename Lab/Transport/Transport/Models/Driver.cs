using System;

namespace Transport.Common.Models
{
    public class Driver
    {

        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string LicenseNumber { get; set; }

        public static string GenerateLicenseNumber()
        {
            return Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }

        public Driver(string firstName, string lastName)
        {
            Id = Guid.NewGuid();
            FirstName = firstName;
            LastName = lastName;
            LicenseNumber = GenerateLicenseNumber(); 
        }

        public string GetFullName()
        {
            return $"{FirstName} {LastName}";
        }
    }
}