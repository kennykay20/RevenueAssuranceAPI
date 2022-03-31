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
        public string DatabasePort { get; set; }
        public string Server { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Status { get; set; }
        public int? UserId { get; set; }
        public string CBSName {get; set;}
        public string CBSVersion { get; set; }
        public string ComplexPassword { get; set; }
        public int MiniPasswordLength { get; set; }   
        public int MaxiPasswordLength { get; set; }  
        public int SpecialCharacter { get; set; }  
        public int NumericNumber { get; set; }   
        public int Uppercase { get; set; }   
    }
}
