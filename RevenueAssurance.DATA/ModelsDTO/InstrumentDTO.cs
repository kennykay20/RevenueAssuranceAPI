using System.Collections.Generic;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class InstrumentDTO
    {
        public string pdtCurrentDate	{get; set;}
         public string psBranchNo {get; set;}
         public string  pnDeptId {get; set;}
         public string pnGlobalView {get; set;}
        public int ServiceId {get; set;}
        public int UserId {get; set;}
         public int? LoginUserId {get; set;}
        public int? DeptId {get; set;}
        public int? RsmId {get; set;}
        public int? RoleId {get; set;}
        public int? MenuId {get; set;}
        public string LoginUserName {get; set;}
        public OprInstrument OprInstrument { get; set;}   
        public List<oprCollateral> ListoprCollateral { get; set;}  
        public List<OprInstrument> ListOprInstrument { get; set;}      
        public List<oprServiceCharges> ListoprServiceCharge{ get; set;}

        public oprInstrmentTemp oprInstrmentTemp { get; set;}

        public List<OprInstrumentDTO> ListOprInstrumentDTO { get; set; }
         public string AcctNo {get; set;}
         public string AcctType {get; set;}
         public string TransAmout {get; set;}
         public string psStatus { get; set; }            
        
        public string CcyCode {get; set;} 

        public string CollateralAcct {get; set;} 
        public string InsuranceAcct {get; set;} 
        public string serialNo { get; set; }
        public string cardNo { get; set; }
        public string TemplateContent { get; set; }
        public string TransactionDate { get; set; }
        public string ValueDate { get; set; }
        public List<RejectionReasonDTO> ListRejectionReasonDTO { get; set;}
        public admAmendReprintReason admAmendReprintReason { get; set;}
        public bool IsAmmendment { get; set; }
        public string AmmendmentOrReprintTxt { get; set; }
        public string BankingDate { get; set; }
        public string referenceNo { get; set; }
        public string Amount { get; set; }
        public string AccountName { get; set; }
        public List<OprAmortizationScheduleDTO> ListOprAmortizationScheduleDTO { get; set;} 
        public string pnCalcAmt {get;set;}

    }
}