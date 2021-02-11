


using System.Threading.Tasks;
using anythingGoodApi.AnythingGood.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.Query;
using RevAssuranceApi.RevenueAssurance.Services.ServiceInterface;

namespace  RevAssuranceApi.RevenueAssurance.Services.ServiceImplementation
{
    public class AuditTrailService : IAuditTrailService
    {
         IQueryUnitOfWork _IQueryUnitOfWork;
        public AuditTrailService(IQueryUnitOfWork IQueryUnitOfWork)
        {
            _IQueryUnitOfWork = IQueryUnitOfWork;
        }

        public async Task<AuditTrail> Get() 
        {
           var get = await  _IQueryUnitOfWork.AuditTrail.GetAsync(c=> c.ItbId  == 1);
           return get;
        }
    }


}