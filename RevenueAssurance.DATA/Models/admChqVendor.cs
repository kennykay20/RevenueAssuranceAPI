

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admChqVendor
    {
         [Key]
        public int ItbId { get; set; }
        public int? VendorId { get; set; }
        public string VendorName { get; set; }
        public string VenAcctNo { get; set; }
        public string VenAcctType { get; set; }
        public string VenEmail { get; set; }
        public string VenAltEmail { get; set; }
        public string VenAcctCcyCode { get; set; }
        public string ChqProductCodes { get; set; }
        public string NotificationType { get; set; }
        public string APIEndPoint { get; set; }
        public string SoapDefinition { get; set; }
        public string Status { get; set; }
        public int? UserId { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
