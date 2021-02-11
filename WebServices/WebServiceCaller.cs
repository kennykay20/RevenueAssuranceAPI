using System.Collections.Generic;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;

namespace RevAssuranceApi.WebServices
{
    public class WebServiceCaller
    {
        WebServiceClient _client;
        IRepository<admBankServiceSetup>  _repoBankServiceSetUp;
       // private readonly IBranchRepository repoBranch;
       // private readonly IUnitOfWork unitOfWork;
        //private readonly IDbFactory idbfactory;
        public WebServiceCaller(IRepository<admBankServiceSetup>  repoBankServiceSetUp, 
        WebServiceClient client)
        {
          //  idbfactory = new DbFactory();
          //  unitOfWork = new UnitOfWork(idbfactory);
           // repoBankServiceSetUp = new BankServiceSetUpRepository(idbfactory);
            _client = client;
            _repoBankServiceSetUp = repoBankServiceSetUp;
        }
        public string GetSOAPResponse(List<WebServiceClient.Parameter> lstParameters, string WebMethod)
        {
            string returnFromService = string.Empty;
            try
            {
                _client = new WebServiceClient
                {
                    WebMethod = WebMethod,
                    Url = "http://localhost/TechClearingSwitch/Service1.asmx",
                    WSServiceType = WebServiceClient.ServiceType.Traditional,
                    Parameters = lstParameters

                };

                returnFromService = _client.InvokeService();
                
            }
            catch (System.Exception ex)
            {
                var exM  = ex;
                //new ErrorLogHandler().SaveLog(string.Format("Error From GetSOAPResponse", ex.Message == null ? ex.InnerException.Message : ex.Message));

            }
            return returnFromService;
        }
   
   
    }
}