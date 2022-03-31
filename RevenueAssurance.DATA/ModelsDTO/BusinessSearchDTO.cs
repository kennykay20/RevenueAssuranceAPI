using System.Collections.Generic;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class BusinessSearchDTO
    {
        public string pdtCurrentDate	{get; set;}
         public string psBranchNo {get; set;}
         public string  pnDeptId {get; set;}
         public string pnGlobalView {get; set;}
        public int ServiceId {get; set;}
        public int UserId {get; set;}

        public int DeptId {get; set;}
        public int RsmId {get; set;}
        public int? RoleId {get; set;}
        public int? MenuId {get; set;}


        public string LoginUserName {get; set;}

        public OprBusinessSearch OprBusinessSearch { get; set;}   
        public List<OprBusinessSearch> ListOprBusinessSearch { get; set;}      
        public List<oprServiceCharges> ListoprServiceCharge{ get; set;}

        public List<OprBusinessSearchDTO> ListOprBusinessSearchDTO { get; set; }
         public string AcctNo {get; set;}
         public string AcctType {get; set;}
         public string TransAmout {get; set;}
        
        public string CcyCode {get; set;} 

        public string CollateralAcct {get; set;} 
        public string InsuranceAcct {get; set;} 
        public int? LoginUserId {get; set;}



         
    }
}