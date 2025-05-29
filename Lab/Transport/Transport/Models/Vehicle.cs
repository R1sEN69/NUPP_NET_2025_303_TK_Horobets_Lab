using System;

namespace Transport.Common.Models
{
    public abstract class Vehicle
    {
        public Guid Id { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public int Year { get; set; }

        public static int VehicleCount { get; private set; }

        static Vehicle()
        {
            VehicleCount = 0;
            Console.WriteLine("Статичний конструктор Vehicle викликано.");
        }

        public Vehicle(string brand, string model, int year)
        {
            Id = Guid.NewGuid();
            Brand = brand;
            Model = model;
            Year = year;
            VehicleCount++; 
            Console.WriteLine($"Конструктор Vehicle викликано. Створено {VehicleCount} транспортних засобів.");
        }

        public virtual string GetInfo()
        {
            return $"ID: {Id}, Марка: {Brand}, Модель: {Model}, Рік: {Year}";
        }

        public void StartEngine()
        {
            Console.WriteLine($"{Brand} {Model} запускає двигун.");
        }

        public delegate void StatusChangedEventHandler(string newStatus);

        public event StatusChangedEventHandler StatusChanged;

        protected void OnStatusChanged(string newStatus)
        {
        
            StatusChanged?.Invoke(newStatus);
        }
    }
}