using System;

class Transport
{
    public string Brand { get; set; }
    public string Model { get; set; }
    public int MaxSpeed { get; set; }

    public Transport(string brand, string model, int maxSpeed)
    {
        Brand = brand;
        Model = model;
        MaxSpeed = maxSpeed;
    }

    public virtual string Info()
    {
        return $"{Brand} {Model} - Максимальна швидкість: {MaxSpeed} км/год";
    }
}

class Car : Transport
{
    public string FuelType { get; set; }
    public int Seats { get; set; }

    public Car(string brand, string model, int maxSpeed, string fuelType, int seats)
        : base(brand, model, maxSpeed)
    {
        FuelType = fuelType;
        Seats = seats;
    }

    public override string Info()
    {
        return base.Info() + $", Тип пального: {FuelType}, Кількість місць: {Seats}";
    }
}

class Bicycle : Transport
{
    public int GearCount { get; set; }
    public string TypeBike { get; set; }

    public Bicycle(string brand, string model, int maxSpeed, int gearCount, string typeBike)
        : base(brand, model, maxSpeed)
    {
        GearCount = gearCount;
        TypeBike = typeBike;
    }

    public override string Info()
    {
        return base.Info() + $", Кількість передач: {GearCount}, Тип: {TypeBike}";
    }
}

class ElectricCar : Car
{
    public int BatteryCapacity { get; set; }
    public int RangeKm { get; set; }

    public ElectricCar(string brand, string model, int maxSpeed, int batteryCapacity, int rangeKm)
        : base(brand, model, maxSpeed, "Electric", 5)
    {
        BatteryCapacity = batteryCapacity;
        RangeKm = rangeKm;
    }

    public override string Info()
    {
        return base.Info() + $", Ємність батареї: {BatteryCapacity} кВт⋅год, Запас ходу: {RangeKm} км";
    }
}

class MountainBike : Bicycle
{
    public string SuspensionType { get; set; }

    public MountainBike(string brand, string model, int maxSpeed, int gearCount, string suspensionType)
        : base(brand, model, maxSpeed, gearCount, "Mountain")
    {
        SuspensionType = suspensionType;
    }

    public override string Info()
    {
        return base.Info() + $", Тип підвіски: {SuspensionType}";
    }
}

class Program
{
    static void Main()
    {
        Car myCar = new Car("Toyota", "Camry", 220, "Petrol", 5);
        ElectricCar myElectricCar = new ElectricCar("Tesla", "Model S", 250, 100, 600);
        Bicycle myBike = new Bicycle("Giant", "Escape 3", 30, 21, "Road");
        MountainBike myMountainBike = new MountainBike("Trek", "Marlin 7", 35, 18, "Full");

        Console.WriteLine(myCar.Info());
        Console.WriteLine(myElectricCar.Info());
        Console.WriteLine(myBike.Info());
        Console.WriteLine(myMountainBike.Info());
    }
}