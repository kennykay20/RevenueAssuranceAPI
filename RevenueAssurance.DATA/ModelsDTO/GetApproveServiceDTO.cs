using System;
namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class GetApproveServiceDTO
    {
            public Int64? ItbId				{ get; set; }
            public Int64? ServiceItbId		{ get; set; }
            public int ServiceId			{ get; set; }
            public string Servicename		{ get; set; }
            public string BranchName		{ get; set; }
            public string AcctNo			{ get; set; }
            public string AcctType			{ get; set; }
            public string AcctName			{ get; set; }
            public string Amount   			{ get; set; }
            public string TellersName		{ get; set; }
            public string ServiceStatus		{ get; set; }
            public DateTime? TransactionDate	{ get; set; }
            public bool Select	{ get; set; }
           
          
    }
}