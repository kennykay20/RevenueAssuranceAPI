

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class OprChqBookRequest
    {
         [Key]
        public int ItbId { get; set; }
        public int? ServiceId { get; set; }
        public int? OrigDeptId { get; set; }
        public string ReferenceNo { get; set; }
        public int? BranchNo { get; set; }
        public string AcctNo { get; set; }
        public string AcctType { get; set; }
        public string AcctName { get; set; }
        public decimal? AvailBal { get; set; }
        public string AcctSic { get; set; }
        public string AcctStatus { get; set; }
        public string CcyCode { get; set; }
        public string ChqProductCode { get; set; }
        public int? ReqBranch { get; set; }
        public int? CollectionBranch { get; set; }
        public int? StartNo { get; set; }
        public int? EndNo { get; set; }
        public int? Quantity { get; set; }
        public string ServiceStatus { get; set; }
        public string Status { get; set; }
        public string OriginatingBranchId { get; set; }
        public int ProcessingDeptId { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? ValueDate { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? UserId { get; set; }
        public int? SupervisorId { get; set; }
        public int? DismissedBy { get; set; }
        public DateTime? DismissedDate { get; set; }
        public int? RejectedBy { get; set; }
        public string Rejected { get; set; }
        public string RejectionIds { get; set; }
        public DateTime? RejectionDate { get; set; }
        public int? VendorId { get; set; }
        public DateTime? DateReceived { get; set; }
        public int? ManfactStartNo { get; set; }
        public int? ManfactEndNo { get; set; }
        public int? EncodedBy { get; set; }
        public DateTime? EncodedDate { get; set; }
        public int? SerializedBy { get; set; }
        public DateTime? SerializedDate { get; set; }
        public int? ChqRangeErrCode { get; set; }
        public string ChqRangeErrText { get; set; }
        public int? RsmId { get; set; }
        public string EncodingStatus { get; set; }
        //public string userName { get; set; }
    }




}
