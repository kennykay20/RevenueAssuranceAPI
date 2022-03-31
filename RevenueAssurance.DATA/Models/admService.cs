

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admService
    {


         [Key]
        public int ServiceId { get; set; }
        public string ServiceDescription { get; set; }
        public int? UserId { get; set; }
        public string ContDrAcctNo { get; set; }
        public string ContDrAcctType { get; set; }
        public string ContDrAcctNarr { get; set; }
        public string ContCrAcctNo { get; set; }
        public string ContCrAcctType { get; set; }
        public string ContCrAcctNarr { get; set; }
        public int? DefaultDept { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Status { get; set; }
        public string IncomeAcctNo { get; set; }
        public string IncomeAcctType { get; set; }
        public string Channel { get; set; }
        public string Frequency { get; set; }
        public int? Sequence { get; set; }
        public string RefPrefix { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string CustAcctDrTC { get; set; }
        public string GLAcctDrTC { get; set; }
        public string CustAcctCrTC { get; set; }
        public string GLAcctCrTC { get; set; }
        public string ReqAmortSched { get; set; }
   
    }
}
