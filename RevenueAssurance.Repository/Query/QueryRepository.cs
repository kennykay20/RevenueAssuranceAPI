using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using anythingGoodApi.AnythingGood.DATA;
using Microsoft.EntityFrameworkCore;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;

namespace  RevAssuranceApi.RevenueAssurance.Repository.Query
{
    
   public class QueryRepository<TEntity> : IQueryRepository<TEntity> where TEntity : class
    {
        private ApplicationDbContext _context;
        private DbSet<TEntity> _dbSet;
        private IUtilities _utilities;

        public QueryRepository(ApplicationDbContext context, IUtilities utilities)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
            _utilities = utilities;
        }

        // public TEntity FindByKey(int id)
        // {
        //     Expression<Func<TEntity, bool>> lambda = _utilities.BuildLambdaForFindByKey<TEntity>(id);
        //     return _dbSet.SingleOrDefault(lambda);
        // }
        public IEnumerable<TEntity> GetAll()
        {
            return _dbSet.ToList();
        }
        public IEnumerable<TEntity> GetMany(Func<TEntity, bool> predicate)
        {
            return _dbSet.Where(predicate).AsQueryable<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }
        public TEntity GetBy()
        {
            return _dbSet.FirstOrDefault();
        }

        public IQueryable<TEntity> GetBy(Func<TEntity, bool> predicate)
        {
            return _dbSet.Where(predicate).AsQueryable<TEntity>();
        }
        public IQueryable<TEntity> GetByNoTracking(Func<TEntity, bool> predicate)
        {
            return _dbSet.AsNoTracking().Where(predicate).AsQueryable<TEntity>();
        }
        public IEnumerable<TEntity> AllInclude(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return GetAllIncluding(includeProperties).ToList();
        }

        public IEnumerable<TEntity> FindByInclude(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = GetAllIncluding(includeProperties);
            IEnumerable<TEntity> results = query.Where(predicate).ToList();
            return results;
        }

        public IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> queryable = _dbSet.AsNoTracking();

            return includeProperties.Aggregate
              (queryable, (current, includeProperty) => current.Include(includeProperty));
        }
         public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where)
        {
            if (where != null)
                return await _dbSet.Where(where).FirstOrDefaultAsync<TEntity>();
            else
                return await _dbSet.SingleOrDefaultAsync();
        }
         public async virtual Task<IEnumerable<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> where)
        {
            return await _dbSet.Where(where).ToListAsync();
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