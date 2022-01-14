
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
    public interface IRepositoryBase<T> where T : BaseEntity
    {
     
        Task<IList<T>> DbGetAllAsync(bool includeDeleted = false);
        Task<T> DbGetByIdAsync(Guid id);

        void Insert(T record);
        void Update(T record);
        void UpdateRange(IList<T> records);
        void Delete(T record);
        void DeleteRange(List<T> records);
        void DetachEntity(T record);
    }

    public class RepositoryBase<T> : IDisposable, IRepositoryBase<T> where T : BaseEntity, new()
    {
        private readonly ApplicationDbContext _db;
        private readonly DbSet<T> _dbSet;

        protected RepositoryBase(ApplicationDbContext db)
        {
            _db = db;
            _dbSet = db.Set<T>();
        }

       
        public async Task<IList<T>> DbGetAllAsync(bool includeDeleted = false)
        {
            return includeDeleted
                ? await _dbSet.IgnoreQueryFilters().ToListAsync()
                : await _dbSet.ToListAsync();
        }
        public async Task<T> DbGetByIdAsync(Guid id)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.Id == id);
        }

        public void Insert(T record)
        {
            if (_db.Entry(record).State == EntityState.Detached)
            {
                _db.Attach(record);
                _db.Entry(record).State = EntityState.Added;
            }
        }
        public void Update(T record)
        {
            if (_db.Entry(record).State == EntityState.Detached)
                _db.Attach(record);
            
            _db.Entry(record).State = EntityState.Modified;
        }

        public void UpdateRange(IList<T> records)
        {
            foreach (var record in records)
            {
                Update(record);
            }
        }
        
        public void Delete(T record)
        {
            if (record != null)
            {
                record.DeletedAt = DateTime.UtcNow;
                Update(record);
            }
        }
      
        public void DeleteRange(List<T> records)
        {
            foreach (var record in records)
            {
                if (record != null)
                {
                    Delete(record);
                }
            }
        }

        /// <summary>
        /// The entity will not be tracked by the EfDbContext and CacheContext
        /// </summary>
        public void DetachEntity(T record)
        {
            if(record != null)
            {
                if(_db.Entry(record).State != EntityState.Detached)
                {
                    _db.Entry(record).State = EntityState.Detached;
                }
            }
        }

        public void Dispose()
        {
            _db?.Dispose();
        }

        protected IQueryable<T> DbGetRecords(bool includeDeleted = false)
        {
            return includeDeleted ? _dbSet.IgnoreQueryFilters() : _dbSet;
        }
        protected IQueryable<T> DbGetRecord(Guid id)
        {
            return _dbSet.Where(e => e.Id == id);
        }

        public void LogDbTrack()
        {
            foreach (var entry in _db.ChangeTracker.Entries()/*.Where(e => e.State == EntityState.Modified)*/)
            {
                Console.WriteLine($"Entity: {entry.Entity.GetType().Name}, State: { entry.State.ToString()}");
            }
        }
    }
}