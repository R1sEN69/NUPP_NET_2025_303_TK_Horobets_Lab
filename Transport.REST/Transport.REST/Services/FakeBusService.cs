using Transport.REST.Interfaces;
using Transport.REST.Models; // Використовуємо наші REST-моделі
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transport.REST.Services
{
    // У реальному проекті тут буде логіка, яка використовує ваш DAL та Repositories
    // Припустимо, що наша BusEntity (з DAL) має ті ж властивості, що і BusModel для простоти
    // В реальності тут була б маппінг між BusModel та BusEntity
    public class FakeBusService : ICrudServiceAsync<BusModel>
    {
        private readonly ConcurrentDictionary<Guid, BusModel> _buses = new ConcurrentDictionary<Guid, BusModel>();

        public FakeBusService()
        {
            // Початкові дані для тестування
            var bus1 = new BusModel { Id = Guid.NewGuid(), PlateNumber = "AA1111AA", Model = "Mercedes Citaro", Capacity = 100 };
            var bus2 = new BusModel { Id = Guid.NewGuid(), PlateNumber = "BB2222BB", Model = "Bogdan A144.5", Capacity = 70 };
            _buses.TryAdd(bus1.Id, bus1);
            _buses.TryAdd(bus2.Id, bus2);
        }

        public Task<bool> CreateAsync(BusModel element)
        {
            if (element.Id == Guid.Empty)
            {
                element.Id = Guid.NewGuid();
            }
            return Task.FromResult(_buses.TryAdd(element.Id, element));
        }

        public Task<BusModel> ReadAsync(Guid id)
        {
            _buses.TryGetValue(id, out var bus);
            return Task.FromResult(bus);
        }

        public Task<IEnumerable<BusModel>> ReadAllAsync()
        {
            return Task.FromResult(_buses.Values.AsEnumerable());
        }

        public Task<IEnumerable<BusModel>> ReadAllAsync(int page, int amount)
        {
            var pagedBuses = _buses.Values.Skip((page - 1) * amount).Take(amount);
            return Task.FromResult(pagedBuses.AsEnumerable());
        }

        public Task<bool> UpdateAsync(BusModel element)
        {
            if (_buses.ContainsKey(element.Id))
            {
                _buses[element.Id] = element;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<bool> RemoveAsync(BusModel element)
        {
            return Task.FromResult(_buses.TryRemove(element.Id, out _));
        }

        public Task<bool> SaveAsync()
        {
            // У цьому фейковому сервісі SaveAsync нічого не робить, оскільки дані в пам'яті
            return Task.FromResult(true);
        }
    }
}