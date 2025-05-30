using System;
using System.Collections.Generic;

namespace Transport.Common.Models
{
    public class Route
    {
        public Guid Id { get; set; }

        public string DeparturePoint { get; set; }

        public string DestinationPoint { get; set; }

        public double DistanceKm { get; set; }

        public Route(string departurePoint, string destinationPoint, double distanceKm)
        {
            Id = Guid.NewGuid();
            DeparturePoint = departurePoint;
            DestinationPoint = destinationPoint;
            DistanceKm = distanceKm;
        }

        public double CalculateApproximateTravelTime(double averageSpeedKmH)
        {
            if (averageSpeedKmH <= 0)
            {
                throw new ArgumentException("Середня швидкість повинна бути більше нуля.");
            }
            return DistanceKm / averageSpeedKmH;
        }
    }
}