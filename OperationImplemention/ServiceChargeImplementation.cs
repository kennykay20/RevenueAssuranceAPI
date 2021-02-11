using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;

namespace RevAssuranceApi.OperationImplemention
{
    public class ServiceChargeImplementation
    {
        IRepository<oprServiceCharges> _repooprServiceCharges;
        ApplicationDbContext _ApplicationDbContext;
        ChargeImplementation _ChargeImplementation;
        IRepository<admStatusItem> _repoStatusItem;
        public ServiceChargeImplementation(IRepository<oprServiceCharges> repooprServiceCharges,
                                            ApplicationDbContext ApplicationDbContext,
                                             ChargeImplementation  ChargeImplementation,
                                             IRepository<admStatusItem> repoStatusItem)
        {
            _repooprServiceCharges = repooprServiceCharges;
            _ApplicationDbContext = ApplicationDbContext;
            _ChargeImplementation = ChargeImplementation;
            _repoStatusItem = repoStatusItem;
        }

        public async Task<int> SaveServiceCharges(oprServiceCharges oprSer, int userId)
        {
            try
            {

                  await _repooprServiceCharges.AddAsync(oprSer);
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

        public async Task<List<oprServiceCharges>> GetServiceCharges(ServiceChargeDTO AnyAuth)
        {
            try
            {

               var get = await _repooprServiceCharges.GetManyAsync(c=> c.ServiceId == AnyAuth.ServiceId && c.ServiceItbId ==  AnyAuth.ServiceItbId);
                 if(get != null)
                 {
                    return get.ToList();

                }
               else
               {
                     var getCharge = await _ChargeImplementation.GetChargeDetails(AnyAuth.ServiceId);
                  
                   var listoprServiceCharges =  new List<oprServiceCharges>();
                   
                    if(getCharge.Count > 0)
                    {
                        foreach(var b in getCharge)
                        {
                           listoprServiceCharges.Add(new oprServiceCharges
                           {

                                ItbId				 = 0,
                                ServiceId			 = (int)b.ServiceId,
                                ServiceItbId		 = 0,
                                ChgAcctNo			 = null,
                                ChgAcctType			 = null,
                                ChgAcctName			 = null,
                                ChgAvailBal			 = null,
                                ChgAcctCcy			 = null,
                                ChgAcctStatus		 = null,
                                ChargeCode			 = b.ChargeCode,
                                ChargeRate			 = null,
                                OrigChgAmount		 = null,
                                OrigChgCCy			 = null,
                                ExchangeRate		 = null,
                                EquivChgAmount		 = null,
                                EquivChgCcy			 = null,
                                ChgNarration		 = null,
                                TaxAcctNo			 = null,
                                TaxAcctType			 = null,
                                TaxRate				 = null,
                                TaxAmount			 = null,
                                TaxNarration		 = null,
                                IncBranch			 = null,
                                IncAcctNo			 = null,
                                IncAcctType			 = null,
                                IncAcctName			 = null,
                                IncAcctBalance		 = null,
                                IncAcctStatus		 = null,
                                IncAcctNarr			 = null,
                                SeqNo				 = null,
                                Status				 = null,
                                DateCreated			 = null,
                          });
                        }
                    }

                     return listoprServiceCharges;

               }
                
            }
            catch (Exception ex)
            {
              //  var msg = await _applicationReturnMessage.returnMessage(20008);
                var exM = ex == null ? ex.InnerException.Message : ex.Message;

                return null;

            }
           

        }

        public async Task<List<admStatusItem>> GetAllStatusItem()
        {
            var get = await _repoStatusItem.GetManyListAsync(x => x.Status != null);
            return get;
        }

        public async Task<List<oprServiceCharges>> GetServiceChargesByServIdAndItbId(int? ServiceId, int ServiceItbId)
        {
            try
            {

               var get = await _repooprServiceCharges.GetManyAsync(c=> c.ServiceId == ServiceId && c.ServiceItbId ==  ServiceItbId);
                return get.ToList();

            }
            catch (Exception ex)
            {
              //  var msg = await _applicationReturnMessage.returnMessage(20008);
                var exM = ex == null ? ex.InnerException.Message : ex.Message;

                return null;

            }
           

        }


    }

  

}