using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using anythingGoodApi.AnythingGood.DATA;
using Microsoft.EntityFrameworkCore;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;

namespace  RevAssuranceApi.RevenueAssurance.Repository.Implementation
{
    
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private ApplicationDbContext  _context;
        private DbSet<TEntity> _dbSet;
        IUtilities _IUtilities;
        public Repository(ApplicationDbContext context, IUtilities IUtilities)
        {
            _context = context;
            _IUtilities = IUtilities;
            _dbSet = context.Set<TEntity>();
        }
        public async Task InsertRangeAsync(IEnumerable<TEntity> entity)
        {
             await  _dbSet.AddRangeAsync(entity);
        }
        
         public async Task AddAsync(TEntity entity)
        {
            await  _dbSet.AddAsync(entity);
        }
       public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where)
        {
            if (where != null)
                return await _dbSet.Where(where).FirstOrDefaultAsync<TEntity>();
            else
                return await _dbSet.SingleOrDefaultAsync();
        }
        
        public void Delete(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            _dbSet.Remove(entity);
        }

        public void InsertRange(List<TEntity> entity) 
        {
            _dbSet.AddRange(entity);

        }

        public void Update(TEntity entity)
        {
            if (entity != null)
            {
                _context.Entry(entity).State = EntityState.Detached;
            }
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
         public async virtual Task<IEnumerable<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> where)
        {
            return await _dbSet.Where(where).ToListAsync();
        }

        public async virtual Task<List<TEntity>> GetManyListAsync(Expression<Func<TEntity, bool>> where)
        {
            return await _dbSet.Where(where).ToListAsync();
        }

        // public void Delete(TEntity entity)
        // {
        //     if (entity == null)
        //     {
        //         throw new ArgumentNullException("entity");
        //     }
        //     _dbSet.Remove(entity);
        // }

        
        public void Delete(Expression<Func<TEntity, bool>> where)
        {
            if (where == null)
            {
                throw new ArgumentNullException("entity");
            }
            IEnumerable<TEntity> objects = _dbSet.Where<TEntity>(where).AsEnumerable();
            foreach (TEntity obj in objects)
                _dbSet.Remove(obj);
        }
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

}