

using System;
using System.Threading.Tasks;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;

namespace RevAssuranceApi.OperationImplemention
{
    public class DocumentRetImplementation
    {
         IRepository<oprDocChgDetail> _repooprDocRetival;
        ApplicationDbContext _ApplicationDbContext;
        ChargeImplementation _ChargeImplementation;
        public DocumentRetImplementation(IRepository<oprDocChgDetail> repooprDocRetival,
                                            ApplicationDbContext ApplicationDbContext,
                                             ChargeImplementation  ChargeImplementation)
        {
            _repooprDocRetival = repooprDocRetival;
            _ApplicationDbContext = ApplicationDbContext;
            _ChargeImplementation = ChargeImplementation;
        }

        public async Task<int> SaveDocRetCharges(oprDocChgDetail oprCharge, int userId)
        {
            try
            {

                  await _repooprDocRetival.AddAsync(oprCharge);
                int Save = await _ApplicationDbContext.SaveChanges(userId);
                if (Save > 0)
                {
                    
                    return 1;
                }

            }
            catch (Exception ex)
            {
              //  var msg = await _applicationReturnMessage.returnMessage(20008);
                var exM = ex == null ? ex.InnerException.Message : ex.Message;

                return -99;

            }
            return -99;

        }

    }

}