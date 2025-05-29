using System;
using System.Linq;
using Transport.Common.Extensions; 
using Transport.Common.Models;
using Transport.Common.Services;

namespace Transport.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; 
            Console.WriteLine("--- Демонстрація транспортної моделі та CRUD сервісу ---");

            Console.WriteLine($"Поточна кількість транспортних засобів: {Vehicle.VehicleCount}");

            Console.WriteLine("\n--- Створення об'єктів ---");
            Car myCar = new Car("Toyota", "Camry", 2020, 4, "Седан");
            Motorcycle myMotorcycle = new Motorcycle("Harley-Davidson", "Iron 883", 2018, "Крузер", false);
            Car anotherCar = new Car("Honda", "Civic", 2022, 5, "Хетчбек");
            Driver driver1 = new Driver("Олександр", "Іваненко");
            Route route1 = new Route("Київ", "Львів", 540);


            Console.WriteLine($"Поточна кількість транспортних засобів після створення: {Vehicle.VehicleCount}");

            myCar.StatusChanged += (status) => Console.WriteLine($"[Подія Car] Статус {myCar.Brand} {myCar.Model}: {status}");
            myMotorcycle.StatusChanged += HandleMotorcycleStatusChange; 

            Console.WriteLine("\n--- Демонстрація методів ---");
            myCar.StartEngine();
            myCar.OpenTrunk();
            myMotorcycle.StartEngine();
            myMotorcycle.PerformStunt("Wheelie");

            Console.WriteLine($"Інформація про машину: {myCar.GetInfo()}");
            Console.WriteLine($"Інформація про мотоцикл: {myMotorcycle.GetInfo()}");
            Console.WriteLine($"Повне ім'я водія: {driver1.GetFullName()}, Номер посвідчення: {driver1.LicenseNumber}");
            Console.WriteLine($"Маршрут: {route1.DeparturePoint} -> {route1.DestinationPoint}, Відстань: {route1.DistanceKm} км");

            Console.WriteLine("\n--- Демонстрація методу розширення ---");
            Console.WriteLine($"Вік моєї машини: {myCar.CalculateAge()} років");
            Console.WriteLine($"Вік мого мотоцикла: {myMotorcycle.CalculateAge()} років");

            Console.WriteLine("\n--- Демонстрація статичного методу ---");
            string newLicense = Driver.GenerateLicenseNumber();
            Console.WriteLine($"Згенерований новий номер посвідчення: {newLicense}");
            Console.WriteLine($"Приблизний час у дорозі для маршруту '{route1.DeparturePoint} - {route1.DestinationPoint}' зі швидкістю 80 км/год: {route1.CalculateApproximateTravelTime(80):F2} годин");


            Console.WriteLine("\n--- Демонстрація CRUD сервісу для Car ---");
            ICrudService<Car> carService = new CrudService<Car>();

            carService.Create(myCar);
            carService.Create(anotherCar);

            Console.WriteLine("\nВсі машини в сервісі:");
            foreach (var car in carService.ReadAll())
            {
                Console.WriteLine(car.GetInfo());
            }

            Guid carIdToRead = myCar.Id;
            Car retrievedCar = carService.Read(carIdToRead);
            if (retrievedCar != null)
            {
                Console.WriteLine($"\nЗнайдена машина за ID {carIdToRead}: {retrievedCar.GetInfo()}");
            }


            Console.WriteLine("\n--- Оновлення машини ---");
            myCar.Brand = "Toyota Updated";
            myCar.Model = "Camry XLE";
            carService.Update(myCar);
            Console.WriteLine($"Оновлена інформація про машину: {carService.Read(myCar.Id)?.GetInfo()}");



            Console.WriteLine("\n--- Видалення машини ---");
            carService.Remove(anotherCar);
            Console.WriteLine("\nВсі машини після видалення:");
            foreach (var car in carService.ReadAll())
            {
                Console.WriteLine(car.GetInfo());
            }

            Console.WriteLine("\n--- Демонстрація CRUD сервісу для Motorcycle ---");
            ICrudService<Motorcycle> motorcycleService = new CrudService<Motorcycle>();
            motorcycleService.Create(myMotorcycle);
            Console.WriteLine($"Мотоцикл у сервісі: {motorcycleService.ReadAll().FirstOrDefault()?.GetInfo()}");

            Console.WriteLine("\n--- Демонстрація CRUD сервісу для Driver ---");
            ICrudService<Driver> driverService = new CrudService<Driver>();
            driverService.Create(driver1);
            Driver driver2 = new Driver("Марія", "Петренко");
            driverService.Create(driver2);
            Console.WriteLine("\nВсі водії в сервісі:");
            foreach (var driver in driverService.ReadAll())
            {
                Console.WriteLine($"{driver.GetFullName()}, Ліцензія: {driver.LicenseNumber}");
            }

            Console.ReadKey(); 
        }


        private static void HandleMotorcycleStatusChange(string newStatus)
        {
            Console.WriteLine($"[Обробник події Motorcycle] Новий статус: {newStatus}");
        }
    }
}