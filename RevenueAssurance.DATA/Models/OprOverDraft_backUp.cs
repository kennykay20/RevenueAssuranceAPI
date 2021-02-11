

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class OprOverDraft_backUp
    {
         [Key]
        public int ItbId { get; set; }
        public int? BatchNo { get; set; }
        public int? ServiceId { get; set; }
        public int? templateType { get; set; }
        public string OdChargeType { get; set; }
        public int? BranchNo { get; set; }
        public string ChgAcctNo { get; set; }
        public string ChgAcctType { get; set; }
        public string ReferenceNo { get; set; }
        public string AcctNo { get; set; }
        public string AcctType { get; set; }
        public string AcctName { get; set; }
        public decimal? OdRate { get; set; }
        public decimal? DrIntRate { get; set; }
        public decimal? UnapprovedOdRate { get; set; }
        public decimal? ApprovedOdRate { get; set; }
        public string OdOption { get; set; }
        public DateTime? OdExpiryDate { get; set; }
        public decimal? Odlimit { get; set; }
        public decimal? ApprovedLimit { get; set; }
        public string ODType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ODTimeBasis { get; set; }
        public int? ODTenor { get; set; }
        public DateTime? ValueDate { get; set; }
        public DateTime? TransDate { get; set; }
        public string IndustrySector { get; set; }
        public string CcyCode { get; set; }
        public int? BatchSeqNo { get; set; }
        public string ChargeCode { get; set; }
        public decimal? ChargeRate { get; set; }
        public decimal? ChargeAmount { get; set; }
        public decimal? TaxAmount { get; set; }
        public string ChargeNarration { get; set; }
        public string Status { get; set; }
        public string ErrorMsg { get; set; }
        public int? DismissedBy { get; set; }
        public DateTime? DismissedDate { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? UserId { get; set; }
        public int? SupervisorId { get; set; }
        public int? ProcessingDept { get; set; }
        public int? InitiatingDept { get; set; }
        public int? IncomeBranch { get; set; }
        public string TransTracer { get; set; }
    }
}
