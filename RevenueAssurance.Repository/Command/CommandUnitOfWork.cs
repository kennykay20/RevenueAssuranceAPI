using System;
using System.Threading.Tasks;
using anythingGoodApi.AnythingGood.DATA;
using anythingGoodApi.AnythingGood.DATA.Models;

using RevAssuranceApi.RevenueAssurance.DATA.Models;

namespace RevAssuranceApi.RevenueAssurance.Repository.Command
{
    
    public class CommandUnitOfWork: ICommandUnitOfWork
    {
         public ICommandRepository<AuditTrail> AuditTrail { get; private set; }
          public ICommandRepository<VehicleInformation> VehicleInformation { get; private set; }
         
        public ApplicationDbContext _context;
         public CommandUnitOfWork(ApplicationDbContext context){
             _context = context;
             AuditTrail = new CommandRepository<AuditTrail>(context);
             VehicleInformation = new CommandRepository<VehicleInformation>(context);
         }
        public async Task<int> Save(int UserId)
        {
            int saved = 0;
            try
            {
                saved = await _context.SaveChanges(UserId);
            }
            catch (Exception ex)
            {
                var exM =  ex == null ? ex.InnerException.Message : ex.Message;
            }

            return saved;
        }
        public void Dispose()
        {
            try
            {
                _context.Dispose();
            }
            catch (Exception ex)
            {
                var excMessage =  ex == null ? ex.InnerException.Message : ex.Message;
            }
        }     
    }


}