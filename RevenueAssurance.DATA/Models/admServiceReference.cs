

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admServiceReference
    {
         [Key]
        public int ItbId { get; set; }
        public int ServiceId { get; set; }
        public string ServiceCode { get; set; }
        public string BankAbreviation { get; set; }
        public string CountryCode { get; set; }
        public string ReferenceNo { get; set; }
        public string Frequency { get; set; }
        public DateTime? Datecreated { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string ReferenceFormat { get; set; }
        public string ReferenceFormatDisplay { get; set; }
        public string Status { get; set; }
        public int? UserId { get; set; }
        
    }


}
