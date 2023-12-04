using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyApps.Infrastructure
{
    public interface IRepository<T> where T : Entity
    {
        int Count { get; }
        Task<T> AddAsync(T newItem);
        Task<T> UpdateAsync(T newItem);
        Task<T> DeleteAsync(Guid id);
        Task Clear();
        Task<T> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
    }
}