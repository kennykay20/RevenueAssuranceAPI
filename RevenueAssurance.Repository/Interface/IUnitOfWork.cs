using System;
using System.Threading.Tasks;
using anythingGoodApi.AnythingGood.DATA.Models;

namespace RevAssuranceApi.RevenueAssurance.Repository.Interface
{
    public interface IUnitOfWork
    {
          IRepository<AuditTrail> AuditTrail { get; set; }
          Task<int> Save(int userid); 
    }
}