using System;
using System.Threading.Tasks;
using anythingGoodApi.AnythingGood.DATA;
using anythingGoodApi.AnythingGood.DATA.Models;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;

namespace  RevAssuranceApi.RevenueAssurance.Repository.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        public IRepository<AuditTrail> AuditTrail { get; set; }
        IUtilities _IUtilities;
        public ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context, IUtilities IUtilities)
        {
            _context = context;   
            _IUtilities = IUtilities;
            AuditTrail = new Repository<AuditTrail>(context, _IUtilities);
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
                var exM = ex == null ?  ex.InnerException.Message  : ex.Message;
              
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