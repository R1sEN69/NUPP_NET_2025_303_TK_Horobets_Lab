using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transport.Infrastructure.Interfaces;
using Transport.Infrastructure.Models;

namespace Transport.Infrastructure.Services
{
    public class CrudServiceAsync<T> : ICrudServiceAsync<T> where T : class
    {
        private readonly IRepository<T> _repository;

        public CrudServiceAsync(IRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<bool> CreateAsync(T element)
        {
            await _repository.AddAsync(element);
            return await _repository.SaveChangesAsync() > 0;
        }

        public async Task<T?> ReadAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<T>> ReadAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
        {
            var allElements = await _repository.GetAllAsync();
            return allElements.Skip((page - 1) * amount).Take(amount);
        }

        public async Task<bool> UpdateAsync(T element)
        {
            await _repository.Update(element);
            return await _repository.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveAsync(T element)
        {
            await _repository.Delete(element);
            return await _repository.SaveChangesAsync() > 0;
        }

        public async Task<bool> SaveAsync()
        {
            return await _repository.SaveChangesAsync() > 0;
        }
    }
}