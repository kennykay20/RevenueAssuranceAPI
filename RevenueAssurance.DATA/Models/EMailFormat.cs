using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
namespace anythingGoodApi.AnythingGood.DATA.Models
{
    public class EMailFormat
    {
         [Key]
        public int AId { get; set; }
        public string SignUpCustomerMailFormat { get; set; }
        public string SignUpCustomerSubject { get; set; }
        public DateTime DateCreated { get; set; }
        public string Remarks { get; set; }
        public string SmtpServer { get; set; }
        public string SmtpPort { get; set; }
    }
}