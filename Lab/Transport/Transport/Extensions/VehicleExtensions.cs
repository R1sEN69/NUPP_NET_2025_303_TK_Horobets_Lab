using Transport.Common.Models; // Потрібно для доступу до класу Vehicle
using System;

namespace Transport.Common.Extensions
{
    // Клас для методів розширення повинен бути статичним
    public static class VehicleExtensions
    {
        // Метод розширення: Додає функціонал для обчислення віку транспортного засобу
        // "this Vehicle vehicle" вказує, що це метод розширення для типу Vehicle
        public static int CalculateAge(this Vehicle vehicle)
        {
            // Метод розширення
            return DateTime.Now.Year - vehicle.Year;
        }
    }
}