

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class oprServiceCharges
    {
         [Key]
        public Int64 ItbId { get; set; }
        public int? ServiceId { get; set; }
        public Int64? ServiceItbId { get; set; }
        public string ChgAcctNo { get; set; }
        public string ChgAcctType { get; set; }
        public string ChgAcctName { get; set; }
        public decimal? ChgAvailBal { get; set; }
        public string ChgAcctCcy { get; set; }
        public string ChgAcctStatus { get; set; }
        public string ChargeCode { get; set; }
        public decimal? ChargeRate { get; set; }
        public decimal? OrigChgAmount { get; set; }
        public string OrigChgCCy { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? EquivChgAmount { get; set; }
        public string EquivChgCcy { get; set; }
        public string ChgNarration { get; set; }
        public string TaxAcctNo { get; set; }
        public string TaxAcctType { get; set; }
        public decimal? TaxRate { get; set; }
        public decimal? TaxAmount { get; set; }
        public string TaxNarration { get; set; }
        public int? IncBranch { get; set; }
        public string IncAcctNo { get; set; }
        public string IncAcctType { get; set; }
        public string IncAcctName { get; set; }
        public string IncAcctBalance { get; set; }
        public string IncAcctStatus { get; set; }
        public string IncAcctNarr { get; set; }
        public int? SeqNo { get; set; }
        public string Status { get; set; }
        public DateTime? DateCreated { get; set; }

        public int? TemplateId { get; set; }
    }
}
