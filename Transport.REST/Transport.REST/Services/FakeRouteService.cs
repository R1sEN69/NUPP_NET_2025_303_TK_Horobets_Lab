using Transport.REST.Interfaces;
using Transport.REST.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transport.REST.Services
{
    public class FakeRouteService : ICrudServiceAsync<RouteModel>
    {
        private readonly ConcurrentDictionary<Guid, RouteModel> _routes = new ConcurrentDictionary<Guid, RouteModel>();

        public FakeRouteService()
        {
            // Початкові дані для тестування
            var route1 = new RouteModel
            {
                Id = Guid.NewGuid(),
                Name = "Маршрут №1А",
                StartPoint = "Центральний Вокзал",
                EndPoint = "Автостанція №2",
                DistanceKm = 15.5m
            };
            var route2 = new RouteModel
            {
                Id = Guid.NewGuid(),
                Name = "Маршрут №5Б",
                StartPoint = "Площа Перемоги",
                EndPoint = "Мікрорайон Сади",
                DistanceKm = 8.2m
            };
            _routes.TryAdd(route1.Id, route1);
            _routes.TryAdd(route2.Id, route2);
        }

        public Task<bool> CreateAsync(RouteModel element)
        {
            if (element.Id == Guid.Empty)
            {
                element.Id = Guid.NewGuid();
            }
            // Перевіряємо, чи вже існує маршрут з таким ID
            if (_routes.ContainsKey(element.Id))
            {
                return Task.FromResult(false); // Не можна створити, якщо ID вже існує
            }
            return Task.FromResult(_routes.TryAdd(element.Id, element));
        }

        public Task<RouteModel> ReadAsync(Guid id)
        {
            _routes.TryGetValue(id, out var route);
            return Task.FromResult(route);
        }

        public Task<IEnumerable<RouteModel>> ReadAllAsync()
        {
            return Task.FromResult(_routes.Values.AsEnumerable());
        }

        public Task<IEnumerable<RouteModel>> ReadAllAsync(int page, int amount)
        {
            if (page < 1) page = 1;
            if (amount < 1) amount = 10; // За замовчуванням 10 елементів на сторінку

            var pagedRoutes = _routes.Values
                                   .OrderBy(r => r.Name) // Для стабільної пагінації
                                   .Skip((page - 1) * amount)
                                   .Take(amount);
            return Task.FromResult(pagedRoutes.AsEnumerable());
        }

        public Task<bool> UpdateAsync(RouteModel element)
        {
            if (_routes.ContainsKey(element.Id))
            {
                _routes[element.Id] = element; // Оновлюємо існуючий елемент
                return Task.FromResult(true);
            }
            return Task.FromResult(false); // Елемент не знайдено для оновлення
        }

        public Task<bool> RemoveAsync(RouteModel element)
        {
            if (element == null)
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(_routes.TryRemove(element.Id, out _));
        }

        public Task<bool> SaveAsync()
        {
            // У цьому фейковому сервісі SaveAsync нічого не робить, оскільки дані в пам'яті
            return Task.FromResult(true);
        }
    }
}