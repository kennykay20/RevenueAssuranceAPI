

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admBank
    {
         [Key]
        public string BankCode { get; set; }
        public string Address { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonEmail { get; set; }
        public string ContactPersonTelephone { get; set; }
        public int UserId { get; set; }
        public DateTime DateCreated { get; set; }
        public string BankName { get; set; }
        public string RoutingNo { get; set; }
        public string ShortName { get; set; }
        public string Status { get; set; }
        public string Telephone { get; set; }
        public string CountryCode { get; set; }
        public string ParticipationCode { get; set; }
        public string SwiftCode { get; set; }
        public string CBNostroAcctNoLcy { get; set; }
        public string CBVostroAcctNoLcy { get; set; }
        public int? AuthorizeId { get; set; }
        public string RtgsFlag { get; set; }
        public bool? MultipleSwiftCodes { get; set; }
    }
}
