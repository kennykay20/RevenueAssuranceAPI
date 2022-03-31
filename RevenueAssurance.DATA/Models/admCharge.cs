

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admCharge
    {
         [Key]
        public int ItbId { get; set; }
        public int? ServiceId { get; set; }
        public string ChargeCode { get; set; }
        public string Description { get; set; }
        public string ChargeType { get; set; }
        public string ChargeBasis { get; set; }
        public decimal? ChargeValue { get; set; }
        public string SubjectToTax { get; set; }
        public int? Direction { get; set; }
        public string CurrencyIso { get; set; }
        public int? UserId { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Status { get; set; }
        public string DRCR { get; set; }
        public string ChargeAcctNo { get; set; }
        public string ChargeAcctType { get; set; }
        public string RecipientBranch { get; set; }
        public decimal? MinimumCharge { get; set; }
        public decimal? MaximumCharge { get; set; }
        public string MinimumChargeCurr { get; set; }
        public string MaximumChargeCurr { get; set; }
        public decimal? ChargeFlat { get; set; }
        public string ChargeFlatCurr { get; set; }
        public string ChargeDescCode { get; set; }
        public string Ammendment { get; set; }
        public string RePrint { get; set; }
        public int? TemplateId { get; set; }
        public string Exempted { get; set; }
        public string PostWithTC { get; set; }
        public string AmortInstAcctType { get; set; }
        public string AmortInstAcctNo { get; set; }
        public decimal? ThresholdAmount { get; set; }
        public string DrAcctAmortNarr { get; set; }
        public string CrAcctAmortNarr { get; set; }
    }
}
