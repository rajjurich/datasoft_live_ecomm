using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core
{
    public interface IEntityRepository<T> where T : class
    {
        Task<T> Add(T entity);
        Task<IEnumerable<T>> AddRange(IEnumerable<T> entities);
        Task<T> Edit(T entity);
        IQueryable<T> GetAll();
        Task<T> GetSingle(int key);
        Task<T> Remove(int key);
        Task Save();
    }
    public class EntityRepository<T> : IEntityRepository<T> where T : class
    {
        readonly DbContext _entitiesContext;
        public EntityRepository(DbContext entitiesContext)
        {
            if (entitiesContext == null)
            {
                throw new ArgumentNullException("entitiesContext");
            }
            _entitiesContext = entitiesContext;
        }

        public async Task<T> Add(T entity)
        {
            DbEntityEntry dbEntityEntry = _entitiesContext.Entry<T>(entity);
            _entitiesContext.Set<T>().Add(entity);
            await Save();
            return entity;
        }

        public async Task<IEnumerable<T>> AddRange(IEnumerable<T> entities)
        {
            DbEntityEntry dbEntityEntry = _entitiesContext.Entry<IEnumerable<T>>(entities);
            _entitiesContext.Set<T>().AddRange(entities);
            await Save();
            return entities;
        }

        public async Task<T> Edit(T entity)
        {
            DbEntityEntry dbEntityEntry = _entitiesContext.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Modified;
            await Save();
            return entity;
        }

        public IQueryable<T> GetAll()
        {
            return _entitiesContext.Set<T>();
        }

        public async Task<T> GetSingle(int key)
        {
            return await _entitiesContext.Set<T>().FindAsync(key);           
        }
        
        public async Task<T> Remove(int key)
        {
            T entity = await GetSingle(key);
            DbEntityEntry dbEntityEntry = _entitiesContext.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Deleted;
            await Save();
            return entity;
        }

        public async Task Save()
        {
            await _entitiesContext.SaveChangesAsync();
        }                
    }
}
