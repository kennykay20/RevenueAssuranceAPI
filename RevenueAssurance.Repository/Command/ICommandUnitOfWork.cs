using System;
using System.Threading.Tasks;
using anythingGoodApi.AnythingGood.DATA.Models;

namespace RevAssuranceApi.RevenueAssurance.Repository.Command
{
      public interface ICommandUnitOfWork:IDisposable
    {
          ICommandRepository<AuditTrail> AuditTrail { get; }

          ICommandRepository<VehicleInformation> VehicleInformation { get; }
          
          Task<int> Save(int UserId);
    }


}