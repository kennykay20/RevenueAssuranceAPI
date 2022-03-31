namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class OperationViewModel
    {
        public int? serviceId {get; set;}
        public string TransAmount {get; set;}
        public string InstrumentAcctNo {get; set;}
        public int TempType {get; set;}
        public int? TempTypeId {get; set;}
        public string InstrumentCcy {get; set;}
        public string ChargeCode {get; set;}
        public string pnCalcAmt {get; set;}

    }
}