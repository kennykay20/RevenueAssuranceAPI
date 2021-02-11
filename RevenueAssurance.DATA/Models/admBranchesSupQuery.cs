

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admBranchesSupQuery
    {
         [Key]
        public int ItbId { get; set; }
        public string Reversal { get; set; }
        public string ApproveTransaction { get; set; }
        public string PostTransaction { get; set; }
        public string AuthorizationList { get; set; }
        public string SalaryList { get; set; }
        public string AuthorizeSalary { get; set; }
        public string AuthorizeAmortization { get; set; }
        public string TransactionEnquiry { get; set; }
        public string AuthorizeInstrument { get; set; }
        public string UnEncodedList { get; set; }
        public string status { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? UserId { get; set; }
    }
}
