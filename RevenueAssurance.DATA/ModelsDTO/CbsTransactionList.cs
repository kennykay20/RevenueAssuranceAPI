namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
  public class CbsTransactionList
{
    public string InstrumentCcy { get; set; }
    public string Beneficiary { get; set; }
    public string EffectiveDate { get; set; }
    public string Limit { get; set; }
    //2020 Apr 02

    public string DetailsUrl { get; set; }
    public string TransTracer { get; set; }
    public string ViewUrl { get; set; }
    public string AcctNo { get; set; }
    public string AcctType { get; set; }
    public string AcctName { get; set; }
    public string BookletType { get; set; }
    public int Quantity { get; set; }
    public string StartNo { get; set; }
    public string EndNo { get; set; }
    public int BranchNo { get; set; }
    public string Chg1Amount { get; set; }
    public string Chg2Amount { get; set; }
    public string id_serviceId { get; set; }
    public int ItbId { get; set; }
    public string DrAcctBranchCode { get; set; }
    public string DrAcctNo { get; set; }
    public string DrAcctType { get; set; }
    public string DrAcctName { get; set; }
    public string DrAcctTC { get; set; }
    public string DrAcctChargeCode { get; set; }
    public string DrAcctChargeAmt { get; set; }
    public string DrAcctTaxAmt { get; set; }
    public string DrAcctChargeNarr { get; set; }
    public string CrAcctBranchCode { get; set; }
    public string CrAcctNo { get; set; }
    public string CrAcctTC { get; set; }
    public string CrAcctType { get; set; }
    public string CrAcctName { get; set; }
    public string CrAcctChargeCode { get; set; }
    public string CrAcctChargeAmt { get; set; }
    public string CrAcctTaxAmt { get; set; }
    public string CrAcctChargeNarr { get; set; }
    public string Amount { get; set; }
    public string Status { get; set; }
    public string CrAcctStatus { get; set; }
    public string DrAcctStatus { get; set; }
    public string TransReference { get; set; }
    public int? BatchId { get; set; }
    public int UserId { get; set; }
    public string CcyCode { get; set; }
    public string DrCcyCode { get; set; }
    public string CrCcyCode { get; set; }
    public string DateCreated { get; set; }
    public string Direction { get; set; }
    public string OriginatingBranchId { get; set; }
    public string Servicename { get; set; }
    public string TellersName { get; set; }
    public string BranchName { get; set; }
    public string DrCr { get; set; }
    public string DeptName { get; set; }
    public int? ServiceId { get; set; }
    public string PostingDate { get; set; }
    public int? DeptId { get; set; }
    public string TransactionDate { get; set; }
    public string Rejected { get; set; }
    public string Channel { get; set; }

    public string branchName { get; set; }
    public string CreatedBy { get; set; }

    public string OrginDept { get; set; }

    public string ItbIdString { get; set; }
    public string TableName { get; set; }

    public string CrAcctBalance { get; set; }
    public string DrAcctBalance { get; set; }

}

}