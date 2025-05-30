using Transport.Common.Models; 
using System;

namespace Transport.Common.Extensions
{
    public static class VehicleExtensions
    {

        public static int CalculateAge(this Vehicle vehicle)
        {
            return DateTime.Now.Year - vehicle.Year;
        }
    }
}
