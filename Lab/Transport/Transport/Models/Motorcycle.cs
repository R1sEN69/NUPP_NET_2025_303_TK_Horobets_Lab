using System;

namespace Transport.Common.Models
{

    public class Motorcycle : Vehicle
    {
        public string MotorcycleType { get; set; }

        public bool HasSidecar { get; set; }

        public Motorcycle(string brand, string model, int year, string motorcycleType, bool hasSidecar)
            : base(brand, model, year)
        {
            MotorcycleType = motorcycleType;
            HasSidecar = hasSidecar;
            Console.WriteLine($"Конструктор Motorcycle викликано: {brand} {model}");
        }

        public override string GetInfo()
        {
            return $"{base.GetInfo()}, Тип: {MotorcycleType}, Коляска: {(HasSidecar ? "Так" : "Ні")}";
        }

        public void PerformStunt(string stuntName)
        {
            Console.WriteLine($"{Brand} {Model} виконує трюк: {stuntName}");
            OnStatusChanged($"Виконано трюк: {stuntName}");
        }
    }
}