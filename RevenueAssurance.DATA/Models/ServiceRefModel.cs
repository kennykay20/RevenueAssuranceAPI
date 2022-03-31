using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    public class ServiceRefModel
    {
         [Key]
        public string sErrorMsg { get; set; }
        public string nReference { get; set; }
        public int nErrorCode { get; set; }
        public int ServiceId { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceDescription { get; set; }
        public string BankAbreviation { get; set; }
        public string CountryCode { get; set; }
        public string ReferenceNo { get; set; }
        public string Frequency { get; set; }
        public string TransactionDate { get; set; }
        public string Servicename { get; set; }
        public string UserId { get; set; }
        public DateTime DateCreated { get; set; }
        public string Status { get; set; }
        public string ReferenceFormat { get; set; }
    }
}
