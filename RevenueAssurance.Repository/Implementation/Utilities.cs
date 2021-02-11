using System;
using System.Linq.Expressions;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;

namespace RevAssuranceApi.RevenueAssurance.Repository.Implementation
{
    public class Utilities : IUtilities
    {
         public Expression<Func<TEntity, bool>> BuildLambdaForFindByKey<TEntity>(int id)
        {
            var item = Expression.Parameter(typeof(TEntity), "entity");
            var prop = Expression.Property(item, "ID");
            var value = Expression.Constant(id);
            var equal = Expression.Equal(prop, value);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(equal, item);
            return lambda;
        }
    }
}