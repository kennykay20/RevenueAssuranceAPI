

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class OprOverDraft
    {
        /* [Key]
        public Int64 ItbId { get; set; }
        public int? ServiceId { get; set; }
        public string AcctNo { get; set; }
        public string AcctType { get; set; }
        public string AcctName { get; set; }
        public string CcyCode { get; set; }
        public decimal? AvailBal { get; set; }
        public string AcctSic { get; set; }
        public string AcctProductCode  { get; set; }
        public string AcctCustNo { get; set; }
        public int? Tenor  { get; set; }
        public string TenorPeriod { get; set; }
        public string AcctStatus { get; set; }
        public DateTime? OdExpiryDate { get; set; }
        public decimal? DrIntRate { get; set; }
        public decimal? Odlimit { get; set; }
        public string OdOption { get; set; }
        public decimal? UnapprovedOdRate { get; set; }
        public string IndustrySector { get; set; }
        public int? BranchNo { get; set; }
        public string ODType { get; set; }
        public DateTime ValueDate { get; set; }
        public DateTime NewExpiryDate { get; set; }
        public decimal? ApprovedLimit { get; set; }
        public decimal? ApprovedOdRate { get; set; }
        public int? originatingBranchId { get; set; }
        public int? BatchNo { get; set; }
        public string ReferenceNo { get; set; }
        public int? OrigDeptId { get; set; }   
        public int? ProcessingDeptId { get; set; } 
        public DateTime? DateCreated { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string ServiceStatus {get; set;}
        public string Status { get; set; }
        public int? UserId { get; set; }
        public int? SupervisorId { get; set; }
        public string Rejected { get; set; }
        public int? DismissedBy { get; set; }
        public DateTime? DismissedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? RejectedBy{get;set;} 
        public string RejectedIds { get; set; }
        public DateTime? RejectedDate { get; set; }
        public string TemplateContentIds {get; set;}
        public int? RsmId {get; set;}
        public string ServiceCcy { get; set;}
        */

        

[Key]
public Int64? ItbId { get; set; }
public int? ServiceId				{get; set; }
public int? OrigDeptId			{get; set; }
public string ReferenceNo			{get; set; }
public string AcctType				{get; set; }
public string AcctNo				{get; set; }
public string AcctName				{get; set; }
public string CcyCode				{get; set; }
public decimal? AvailBal				{get; set; }
public string AcctSic				{get; set; }
public string AcctStatus			{get; set; }
public int? BranchNo				{get; set; }
public string AcctProductCode		{get; set; }
public string AcctCustNo			{get; set; }
public DateTime? OdExpiryDate			{get; set; }
public decimal? DrIntRate				{get; set; }
public decimal? Odlimit				{get; set; }
public string OdOption				{get; set; }
public decimal? UnapprovedOdRate		{get; set; }
public int? ProcessingDeptId		{get; set; }
public string ODType				{get; set; }
public DateTime? StartDate				{get; set; }
public int? Tenor					{get; set; }
public string TenorPeriod			{get; set; }
public DateTime? NewExpiryDate			{get; set; }
public decimal? ApprovedLimit			{get; set; }
public decimal? ApprovedOdRate		{get; set; }
public string ServiceCcy			{get; set; }
public int? RsmId					{get; set; }
public string Rejected				{get; set; }
public DateTime? RejectedDate			{get; set; }
public Int64? ParentId				{get; set; }
public string ServiceStatus			{get; set; }
public DateTime? TransactionDate		{get; set; }
public DateTime? ValueDate				{get; set; }
public DateTime? DateCreated			{get; set; }
public string Status				{get; set; }
public int? UserId				{get; set; }
public int? SupervisorId			{get; set; }
public DateTime? ApprovalDate			{get; set; }
public int? OriginatingBranchId	{get; set; }
public DateTime? AmortDate				{get; set; }
public string RejectedIds			{get; set; }
public int? RejectedBy			{get; set; }
public string IndustrySector		{get; set; }
public int? BatchNo				{get; set; }
public int? DismissedBy			{get; set; }
public DateTime? DismissedDate			{get; set; }

public string TemplateContentIds	{get; set; }
    }


}
