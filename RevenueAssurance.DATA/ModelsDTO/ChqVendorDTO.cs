using System;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class ChqVendorDTO
    {
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

        //to hold notification types
        public string Email { get; set; }

        public string Api { get; set; }
        public string Webservice { get; set; }

        public string Twenty {get; set;}
        public string Fifty { get; set; }
        public string Hundred { get; set; }    
    }
}