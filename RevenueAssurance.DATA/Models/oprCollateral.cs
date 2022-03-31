

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class oprCollateral
    {
         [Key]
        public Int64? ItbId { get; set; }
        public int? ServiceId { get; set; }
        public Int64? ServiceItbId { get; set; }
        public int CollTypeId { get; set; }
        public string CollDescription { get; set; }
        public string AcctNo { get; set; }
        public string AcctType { get; set; }
        public decimal? AvailBal { get; set; }
        public string AcctName { get; set; }
        public string AcctStatus { get; set; }
        public string PlaceHold  { get; set; }
        public long? HoldId { get; set; }
        public decimal? LienAmount { get; set; }
        public string CollStatus { get; set; }
        public decimal? ForcedSaleValue { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Location { get; set; }
        public string AcctCCy { get; set; }
        public string Valuer { get; set; }
        public string VerifiedBy { get; set; }
        public DateTime? VerificationDate { get; set; }
        public decimal? MarketValue { get; set; }
        public string InsCoyName { get; set; }
        public int? Insured { get; set; }

        public int? InsCoverTypeId { get; set; }

        public string PolicyNo { get; set; }

        public decimal? SumAssured { get; set; } 

         public decimal? PremiumPayable { get; set; }
        public string CollMortgageNo { get; set; }
        public int? UserId { get; set; }
        public string Status { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
