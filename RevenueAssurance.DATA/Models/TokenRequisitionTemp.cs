

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class TokenRequisitionTemp
    {
         [Key]
        public decimal ItbId { get; set; }
        public string AcctNumber { get; set; }
        public string AcctType { get; set; }
        public string AcctCbsTc { get; set; }
        public string AcctNarration { get; set; }
        public decimal? Amount { get; set; }
        public string Reference { get; set; }
        public decimal? ChargeAmount { get; set; }
        public string ChargeCode { get; set; }
        public decimal? TaxAmount { get; set; }
        public string Channel { get; set; }
        public string Status { get; set; }
        public string ErrorMsg { get; set; }
        public int? UserId { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? BatchId { get; set; }
        public string Currency { get; set; }
        public int? NoOfTokens { get; set; }
    }
}
