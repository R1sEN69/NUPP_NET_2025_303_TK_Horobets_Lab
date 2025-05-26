using System.Collections.Generic;
using System.Threading.Tasks;

namespace Transport.Infrastructure.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task Update(T entity);
        Task Delete(T entity);
        Task<int> SaveChangesAsync();
    }
}