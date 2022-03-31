

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admTransactionConfiguration
    {
         [Key]
        public int ItbId { get; set; }
        public int ServiceId { get; set; }
        public short? Direction { get; set; }
        public string CustomerAcctDrTC { get; set; }
        public string GLAcctDrTC { get; set; }
        public string DrNarration { get; set; }
        public string DrChargeNarr { get; set; }
        public string CustomerAcctCrTC { get; set; }
        public string GLAcctCrTC { get; set; }
        public string CrNarration { get; set; }
        public string CrChargeNarr { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? UserId { get; set; }
        public string GLAcctCrTCRev { get; set; }
        public string GLAcctDrTCRev { get; set; }
        public string DebitAcctTCRev { get; set; }
        public string CreditAcctTCRev { get; set; }
        public string Status { get; set; }
        public int? ChargeType { get; set; }
        public string SubjectToAuthorization { get; set; }
    }
}
