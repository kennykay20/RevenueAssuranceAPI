using System.Collections.Generic;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class ServiceChargeDTO
    {
        public int ItbId					{ get; set; }
        public int ServiceId				{ get; set; }
        public int ServiceItbId			{ get; set; }
        public string ChgAcctNo				{ get; set; }
        public string ChgAcctType			{ get; set; }
        public string ChgAcctName			{ get; set; }
        public string ChgAvailBal			{ get; set; }
        public string ChgAcctCcy			{ get; set; }
        public string ChgAcctStatus			{ get; set; }
        public string ChargeCode			{ get; set; }
        public string ChargeRate			{ get; set; }
        public string OrigChgAmount			{ get; set; }
        public string OrigChgCCy			{ get; set; }
        public string ExchangeRate			{ get; set; }
        public string EquivChgAmount		{ get; set; }
        public string EquivChgCcy			{ get; set; }
        public string ChgNarration			{ get; set; }
        public string TaxAcctNo				{ get; set; }
        public string TaxAcctType			{ get; set; }
        public string TaxRate				{ get; set; }
        public string TaxAmount				{ get; set; }
        public string TaxNarration			{ get; set; }
        public string IncBranch				{ get; set; }
        public string IncAcctNo				{ get; set; }
        public string IncAcctType			{ get; set; }
        public string IncAcctName			{ get; set; }
        public string IncAcctBalance		{ get; set; }
        public string IncAcctStatus			{ get; set; }
        public string IncAcctNarr			{ get; set; }
        public string SeqNo					{ get; set; }
        public string Status				{ get; set; }
        public string DateCreated			{ get; set; }
        public int UserId			{ get; set; }
        public List<oprServiceCharges> ListoprServiceCharge{ get; set;}
    }


    public class statusItemDTO{
        public int itbid { get; set; }
        public string StatusValue { get; set; }
        public string Status  { get; set; }
    }

}

