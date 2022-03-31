using System.Collections.Generic;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class CardDTO
    {
        public string pdtCurrentDate	{get; set;}
         public string psBranchNo {get; set;}
         public string  pnDeptId {get; set;}
         public string pnGlobalView {get; set;}
        public int ServiceId {get; set;}
        public int UserId {get; set;}
        public int DeptId {get; set;}
        public int RsmId {get; set;}

        public string LoginUserName {get; set;}

         public OprCard OprCard { get; set;}   
        public List<OprCard> ListOprCard { get; set;}      
        public List<oprServiceCharges> ListoprServiceCharge{ get; set;}

        public List<OprCardDTO> ListOprCardDTO { get; set; }
         public string AcctNo {get; set;}
         public string AcctType {get; set;}
         public string TransAmout {get; set;}
        
        public string CcyCode {get; set;} 
         
    }
}