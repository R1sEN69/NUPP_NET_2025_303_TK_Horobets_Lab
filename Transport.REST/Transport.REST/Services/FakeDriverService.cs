using Transport.REST.Interfaces;
using Transport.REST.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transport.REST.Services
{
    public class FakeDriverService : ICrudServiceAsync<DriverModel>
    {
        private readonly ConcurrentDictionary<Guid, DriverModel> _drivers = new ConcurrentDictionary<Guid, DriverModel>();

        public FakeDriverService()
        {
            // Початкові дані для тестування
            var driver1 = new DriverModel
            {
                Id = Guid.NewGuid(),
                FirstName = "Іван",
                LastName = "Петров",
                LicenseNumber = "АА123456",
                DateOfBirth = new DateTime(1980, 5, 10)
            };
            var driver2 = new DriverModel
            {
                Id = Guid.NewGuid(),
                FirstName = "Олена",
                LastName = "Сидорова",
                LicenseNumber = "ВВ789012",
                DateOfBirth = new DateTime(1992, 11, 25)
            };
            _drivers.TryAdd(driver1.Id, driver1);
            _drivers.TryAdd(driver2.Id, driver2);
        }

        public Task<bool> CreateAsync(DriverModel element)
        {
            if (element.Id == Guid.Empty)
            {
                element.Id = Guid.NewGuid();
            }
            // Перевіряємо, чи вже існує водій з таким ID
            if (_drivers.ContainsKey(element.Id))
            {
                return Task.FromResult(false); // Не можна створити, якщо ID вже існує
            }
            return Task.FromResult(_drivers.TryAdd(element.Id, element));
        }

        public Task<DriverModel> ReadAsync(Guid id)
        {
            _drivers.TryGetValue(id, out var driver);
            return Task.FromResult(driver);
        }

        public Task<IEnumerable<DriverModel>> ReadAllAsync()
        {
            return Task.FromResult(_drivers.Values.AsEnumerable());
        }

        public Task<IEnumerable<DriverModel>> ReadAllAsync(int page, int amount)
        {
            if (page < 1) page = 1;
            if (amount < 1) amount = 10; // За замовчуванням 10 елементів на сторінку

            var pagedDrivers = _drivers.Values
                                     .OrderBy(d => d.LastName) // Для стабільної пагінації
                                     .Skip((page - 1) * amount)
                                     .Take(amount);
            return Task.FromResult(pagedDrivers.AsEnumerable());
        }

        public Task<bool> UpdateAsync(DriverModel element)
        {
            if (_drivers.ContainsKey(element.Id))
            {
                _drivers[element.Id] = element; // Оновлюємо існуючий елемент
                return Task.FromResult(true);
            }
            return Task.FromResult(false); // Елемент не знайдено для оновлення
        }

        public Task<bool> RemoveAsync(DriverModel element)
        {
            if (element == null)
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(_drivers.TryRemove(element.Id, out _));
        }

        public Task<bool> SaveAsync()
        {
            // У цьому фейковому сервісі SaveAsync нічого не робить, оскільки дані в пам'яті
            return Task.FromResult(true);
        }
    }
}