using System;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class OprAcctStatmentDTO
    {
        public Int64 ItbId { get; set; }
        public string SameChrgeAcct { get; set; }
        public int? CoreBankingType { get; set; }
        public int? ServiceId { get; set; }
        public int? BranchNo { get; set; }
        public string ReferenceNo { get; set; }
        public string AcctNo { get; set; }
        public string AcctType { get; set; }
        public string AcctName { get; set; }
        public string AvailBal { get; set; }
        public string AcctSic { get; set; }
         public string AcctStatus { get; set; }
        public DateTime? FromDate { get; set; }
        public int? RsmId { get; set; }
        public DateTime? ToDate { get; set; }
        public int? NoOfPages { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string Embassy { get; set; }
        public string OtherReason { get; set; }
        public DateTime? ValueDate { get; set; }
        public DateTime? TransDate { get; set; }
        public string IndustrySector { get; set; }
        public string CcyCode { get; set; }
        public int? OrigDeptId { get; set; }
        public string Status { get; set; }
        public string ServiceStatus { get; set; }
        public int? DismissedBy { get; set; }
        public DateTime? DismissedDate { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? UserId { get; set; }
        public int? SupervisorId { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate  { get; set; }
        public int? RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }
        public int? RejectedIds { get; set; }
        public string AcctProductCode { get; set; }
        public int? ProcessingDeptId { get; set; } 
         public int? OriginatingBranchId  { get; set; } 
         public string TemplateContentIds { get; set; }
        public string UserName { get; set; } 
        public string OriginBranch { get; set; } 
        public string userProcessingDept { get; set; }
        public string originBranchName { get; set; }

    }

}