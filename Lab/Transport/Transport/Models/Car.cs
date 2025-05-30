using System;

namespace Transport.Common.Models
{

    public class Car : Vehicle
    {

        public int NumberOfDoors { get; set; }

        public string BodyType { get; set; }

        public Car(string brand, string model, int year, int numberOfDoors, string bodyType)
            : base(brand, model, year)
        {
            NumberOfDoors = numberOfDoors;
            BodyType = bodyType;
            Console.WriteLine($"Конструктор Car викликано: {brand} {model}");
        }

        public override string GetInfo()
        {
            return $"{base.GetInfo()}, Двері: {NumberOfDoors}, Кузов: {BodyType}";
        }

        public void OpenTrunk()
        {
            Console.WriteLine($"{Brand} {Model} відкриває багажник.");
            OnStatusChanged("Багажник відкрито");
        }
    }
}