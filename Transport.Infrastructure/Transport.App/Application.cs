using Microsoft.EntityFrameworkCore; // Потрібно для MigrateAsync
using System;
using System.Linq;
using System.Threading.Tasks;
using Transport.Infrastructure; // Для TransportContext (якщо він у цьому namespace)
using Transport.Infrastructure.Interfaces;
using Transport.Infrastructure.Models; // Для CarModel

namespace Transport.App // Це простір імен вашого головного проекту
{
    // Клас Application містить основну логіку програми
    public class Application
    {
        private readonly ICrudServiceAsync<CarModel> _carService;
        private readonly TransportContext _context;

        // Залежності вводяться через конструктор за допомогою Dependency Injection
        public Application(ICrudServiceAsync<CarModel> carService, TransportContext context)
        {
            _carService = carService;
            _context = context;
        }

        // Метод Run() містить послідовність операцій вашої програми
        public async Task Run()
        {
            try
            {
                // Виконання міграцій бази даних
                await _context.Database.MigrateAsync();
                Console.WriteLine("База даних успішно мігрована/перевірена.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при міграції бази даних: {ex.Message}");
                Console.WriteLine("Будь ласка, переконайтесь, що база даних створена та доступна.");
                // Не завершуємо програму, якщо міграція не вдалася, а просто повертаємося
                return;
            }

            Console.WriteLine("База даних готова. Починаємо роботу з сервісом.");

            // Демонстрація створення нового автомобіля
            var newCar = new CarModel
            {
                Brand = "Tesla",
                Model = "Model 3",
                Year = 2023,
                MaxSpeed = 250
            };
            bool created = await _carService.CreateAsync(newCar);
            if (created)
            {
                Console.WriteLine($"Додано новий автомобіль: {newCar.Brand} {newCar.Model}");
            }
            else
            {
                Console.WriteLine("Не вдалося додати автомобіль.");
            }

            // Демонстрація читання всіх автомобілів
            Console.WriteLine("\nВсі автомобілі:");
            var cars = await _carService.ReadAllAsync();
            if (cars.Any())
            {
                foreach (var car in cars)
                {
                    Console.WriteLine($"- ID: {car.Id}, Brand: {car.Brand}, Model: {car.Model}, Year: {car.Year}, MaxSpeed: {car.MaxSpeed}");
                }
            }
            else
            {
                Console.WriteLine("Автомобілів не знайдено.");
            }

            // Демонстрація оновлення автомобіля
            var carToUpdate = cars.FirstOrDefault(c => c.Brand == "Tesla");
            if (carToUpdate != null)
            {
                Console.WriteLine($"\nСпроба оновити автомобіль: {carToUpdate.Brand} {carToUpdate.Model}");
                carToUpdate.MaxSpeed = 260;
                bool updated = await _carService.UpdateAsync(carToUpdate);
                if (updated)
                {
                    Console.WriteLine($"Оновлено MaxSpeed для {carToUpdate.Brand} {carToUpdate.Model} до {carToUpdate.MaxSpeed}");
                }
                else
                {
                    Console.WriteLine("Не вдалося оновити автомобіль.");
                }
            }
            else
            {
                Console.WriteLine("Автомобіль 'Tesla' для оновлення не знайдено.");
            }

            // Демонстрація видалення автомобіля
            var carToRemove = cars.FirstOrDefault(c => c.Brand == "Tesla"); // Знову шукаємо, якщо треба видалити
            if (carToRemove != null)
            {
                Console.WriteLine($"\nСпроба видалити автомобіль: {carToRemove.Brand} {carToRemove.Model}");
                bool removed = await _carService.RemoveAsync(carToRemove);
                if (removed)
                {
                    Console.WriteLine($"Видалено {carToRemove.Brand} {carToRemove.Model}");
                }
                else
                {
                    Console.WriteLine("Не вдалося видалити автомобіль.");
                }
            }
            else
            {
                Console.WriteLine("Автомобіль 'Tesla' для видалення не знайдено.");
            }

            Console.WriteLine("\nЗавершено. Натисніть будь-яку клавішу для виходу.");
            Console.ReadKey();
        }
    }
}