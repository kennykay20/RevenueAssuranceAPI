using System.Collections.Generic;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class TokenDTO
    {
        public string pdtCurrentDate	{get; set;}
        public string transactionDate { get; set; }
         public string psBranchNo {get; set;}
         public string  pnDeptId {get; set;}
         public string pnGlobalView {get; set;}
         public string ReferenceNo { get; set; }
         public string AcctName { get; set; }
        public int ServiceId {get; set;}
        public int UserId {get; set;}
        public int DeptId {get; set;}
        public int RsmId {get; set;}
        public string status { get; set; }
        public string LoginUserName {get; set;}

         public OprToken OprToken { get; set;}   
        public List<OprToken> ListOprToken { get; set;}      
        public List<oprServiceCharges> ListoprServiceCharge{ get; set;}

        public List<OprTokenDTO> ListOprTokenDTO { get; set; }
         public string AcctNo {get; set;}
         public string AcctType {get; set;}
         public string TransAmout {get; set;}
        
        public string CcyCode {get; set;} 

        public long ItbId { get; set; } 
        public int? MenuId {get; set;}
        public int? RoleId {get; set;}
         
    }
}