namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class UploadBulkFilesValidator
    {
    public string AcctNo { get; set; }
    public string AcctType { get; set; }
    public string CbsTC { get; set; }
    public string Narration { get; set; }              
    public string Amount { get; set; }
    public string DrCr { get; set; }
    public string ChargeCode { get; set; }
    public string ChargeAmount  { get; set; }
    public string ChgNarration  { get; set; }
    public string ValueDate { get; set; }
    public string ChequeNo { get; set; }
    public string CcyCode { get; set; }
    public string ValErrorMessage { get; set;}

    public bool? DontSave { get; set;}

  }
}