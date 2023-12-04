using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApps.Infrastructure
{
    public abstract class MemoryRepository<T> : IRepository<T> where T : Entity
    {
        protected MemoryStore<T> _store = new();

        public int Count => _store.Count;

        public virtual Task<T> AddAsync(T newItem)
        {
            _store.Add(newItem.Id, newItem);
            return Task.FromResult(newItem);
        }

        public virtual Task<T> UpdateAsync(T newItem)
        {
            _store[newItem.Id] = newItem;
            return Task.FromResult(newItem);
        }

        public virtual Task<T> DeleteAsync(Guid id)
        {
            var byId = GetByIdAsync(id).Result;
            _store.Remove(byId.Id);
            return Task.FromResult(byId);
        }

        public virtual Task Clear()
        {
            _store.Clear();
            return Task.CompletedTask;
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            var entity = _store.TryGetValue(id, out var value) ? value : null;
            return Task.FromResult(entity);
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(_store.Values.ToList().AsEnumerable());
        }
    }
}
