using System;

public class Bus : BaseEntity
{
    public string Model { get; set; }
    public int Year { get; set; }
    public double FuelConsumptionPer100Km { get; set; }
    public int PassengerCapacity { get; set; }

    public static Bus CreateNew()
    {
        var random = new Random();
        return new Bus
        {
            Model = $"Bus_Model_{Guid.NewGuid().ToString().Substring(0, 4)}",
            Year = random.Next(1990, DateTime.Now.Year + 1),
            FuelConsumptionPer100Km = Math.Round(random.NextDouble() * (30.0 - 15.0) + 15.0, 2), // 15.0 - 30.0 L/100km
            PassengerCapacity = random.Next(20, 100)
        };
    }

    public override string ToString()
    {
        return $"Id: {Id}, Model: {Model}, Year: {Year}, Fuel Consumption: {FuelConsumptionPer100Km} L/100km, Capacity: {PassengerCapacity}";
    }
}