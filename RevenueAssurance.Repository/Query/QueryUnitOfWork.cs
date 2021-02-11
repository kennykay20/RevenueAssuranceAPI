using anythingGoodApi.AnythingGood.DATA;
using anythingGoodApi.AnythingGood.DATA.Models;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;

namespace  RevAssuranceApi.RevenueAssurance.Repository.Query
{
    public class QueryUnitOfWork: IQueryUnitOfWork
    {
         public IQueryRepository<AuditTrail> AuditTrail { get; private set; }

          IUtilities _utilities;
          public ApplicationDbContext _context;
         public QueryUnitOfWork(ApplicationDbContext context, IUtilities utilities)
        {
             _context = context;
             _utilities = utilities;
              AuditTrail = new QueryRepository<AuditTrail>(context, _utilities);
        }
    }


}