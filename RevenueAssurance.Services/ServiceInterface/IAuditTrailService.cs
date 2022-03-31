using System.Threading.Tasks;
using anythingGoodApi.AnythingGood.DATA.Models;

namespace RevAssuranceApi.RevenueAssurance.Services.ServiceInterface
{
    public interface IAuditTrailService
    {
         Task<AuditTrail> Get();
    }
}