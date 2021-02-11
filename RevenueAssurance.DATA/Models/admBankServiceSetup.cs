using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    public class admBankServiceSetup
    {
         [Key]
        public short Itbid { get; set; }
        public string WebServiceUrl { get; set; }
        public string ConnectionName { get; set; }
        public string DataBaseName { get; set; }
        public short? DatabasePort { get; set; }
        public string Server { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Status { get; set; }
        public int? UserId { get; set; }
    }
}
