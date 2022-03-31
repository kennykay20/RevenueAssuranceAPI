using System.ComponentModel.DataAnnotations;

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    public class ServicesModel
    {
         [Key]
        public int TokenRequisition { get; set; }
        public int ChequeBookRequest { get; set; }
        public int CounterChequeRequest { get; set; }
        public int StopChequeRequest { get; set; }
        public int BusinessSearchReport { get; set; }
        public int AccountClosure { get; set; }
        public int CardRequest { get; set; }
        public int AccountStatement { get; set; }
        public int TradeReference { get; set; }
        public int ReferenceLetter { get; set; }
        public int BidSecurity { get; set; }
        public int SalaryProcessing { get; set; }
        public int OverDraftManagement { get; set; }
        public int BankGuarantee { get; set; }
        public int Amortization { get; set; }
        public int Others { get; set; }
        public int Loans { get; set; }
        public int PerformanceBond { get; set; }
        public int AdvancePayGuarantee { get; set; }
    }
}