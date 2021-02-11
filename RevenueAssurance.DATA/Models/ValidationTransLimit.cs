using System.ComponentModel.DataAnnotations;

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    public class ValidationTransLimit
    {
        [Key]
        public bool CreditLimit { get; set; }
        public bool DebitLimit { get; set; }
        public bool GLDebitLimit { get; set; }
        public bool GLCreditLimit { get; set; }
    }
}