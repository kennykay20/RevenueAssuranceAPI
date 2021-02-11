using System;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    
    public class OprStopChqRequestDTO
    {

    
public int ItbId					{get; set; }			
public int ServiceId				{get; set; }
public int BranchNo				{get; set; }
public string ChgAcctNo				{get; set; }
public string ChgAcctType			{get; set; }
public string ReferenceNo			{get; set; }
public string AcctNo				{get; set; }
public string AcctType				{get; set; }
public string AcctName				{get; set; }
public string HitOption				{get; set; }
public long ChqNoFrom				{get; set; }
public long ChqNoTo				{get; set; }
public decimal ChqAmt				{get; set; }
public DateTime ChqDate				{get; set; }
public string Beneficiary			{get; set; }
public string Reason				{get; set; }
public DateTime TransactionDate		{get; set; }
public DateTime ValueDate				{get; set; }
public string IndustrySector		{get; set; }
public string CcyCode				{get; set; }
public string Status				{get; set; }
public string ErrorMsg				{get; set; }
public int? DismissedBy			{get; set; }
public string DismissedDate			{get; set; }
public string DateCreated			{get; set; }
public int? UserId				{get; set; }
public int? SupervisorId			{get; set; }
public int? ApprovedBy			{get; set; }
public string Rejected				{get; set; }
public string RejectionIds			{get; set; }
public string RejectionDate			{get; set; }
public int? RejectedBy			{get; set; }
public string ServiceStatus			{get; set; }
public decimal AvailBal				{get; set; }
public string AcctStatus			{get; set; }
public int? OriginatingBranchId	{get; set; }
public int? ProcessingDeptId		{get; set; }
public int? RsmId					{get; set; }
public int? OrigDeptId			{get; set; }
public string UserName {get; set; }
    }



}