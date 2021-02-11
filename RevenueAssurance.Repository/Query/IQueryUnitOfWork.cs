using anythingGoodApi.AnythingGood.DATA.Models;

namespace  RevAssuranceApi.RevenueAssurance.Repository.Query
{
    
    public interface IQueryUnitOfWork
    {   
        IQueryRepository<AuditTrail> AuditTrail { get; } 
               
    }


}