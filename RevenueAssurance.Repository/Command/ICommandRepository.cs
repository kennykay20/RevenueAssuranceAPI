using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RevAssuranceApi.RevenueAssurance.Repository.Command
{
    
    public interface ICommandRepository<TEntity> : IDisposable where TEntity : class
    {
        void Insert(TEntity entity);
        void InsertRange(List<TEntity> entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        void Delete(Expression<Func<TEntity, bool>> where);
        // Task<IdentityResult> RegisterUser(ApplicationUser applicationUser);
        // int Save();
    }


}