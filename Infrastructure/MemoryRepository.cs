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

    public virtual async Task<T> AddAsync(T newItem)
    {
        _store.Add(newItem.Id, newItem);
        return await Task.FromResult(newItem);
    }

    public virtual async Task<T> UpdateAsync(T newItem)
    {
        _store[newItem.Id] = newItem;
        return await Task.FromResult(newItem);
    }

    public virtual async Task<T> DeleteAsync(Guid id)
    {
        var byId = await GetByIdAsync(id);
        _store.Remove(byId.Id);
        return await Task.FromResult(byId);
    }

    public virtual async Task Clear()
    {
        _store.Clear();
        await Task.CompletedTask;
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        var entity = _store.GetValueOrDefault(id);
        return await Task.FromResult(entity);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await Task.FromResult(_store.Values.ToList().AsEnumerable());
    }
}
}
