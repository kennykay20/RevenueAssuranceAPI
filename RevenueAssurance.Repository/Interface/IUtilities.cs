using System;
using System.Linq.Expressions;

namespace RevAssuranceApi.RevenueAssurance.Repository.Interface
{
    public interface IUtilities
    {
       Expression<Func<TEntity, bool>> BuildLambdaForFindByKey<TEntity>(int id);
    }
}