

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admChequeProduct
    {
         [Key]
        public int ItbId { get; set; }
        public string ChqProductCode { get; set; }
        public string ChqProductDescr { get; set; }
        public int UserId { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Status { get; set; }
        public int? VendorId { get; set; }
        public int? ProductId { get; set; }
        public int? NoOfChqPerUnit { get; set; }
        public string Unit { get; set; }
    }
}
