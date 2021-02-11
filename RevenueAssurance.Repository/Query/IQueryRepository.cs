using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
namespace  RevAssuranceApi.RevenueAssurance.Repository.Query
{
        public interface IQueryRepository<TEntity>: IDisposable where TEntity : class
    {
        //TEntity FindByKey(int id);
        IEnumerable<TEntity> GetMany(Func<TEntity, bool> predicate);
        IEnumerable<TEntity> GetAll();
        Task<IEnumerable<TEntity>> GetAllAsync();
        IQueryable<TEntity> GetBy(Func<TEntity, bool> predicate);
        IQueryable<TEntity> GetByNoTracking(Func<TEntity, bool> predicate);
        TEntity GetBy();
        IEnumerable<TEntity> AllInclude(params Expression<Func<TEntity, object>>[] includeProperties);

        IEnumerable<TEntity> FindByInclude(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties);

        IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties);

        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where);
        Task<IEnumerable<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> where);
    }

}