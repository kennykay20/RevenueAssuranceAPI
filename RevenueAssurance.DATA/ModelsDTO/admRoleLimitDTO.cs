using System;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{

    public class admRoleLimitDTO
    {
        
        public int? ItbId { get; set; }
        public int? RoleId { get; set; }
        public string CurrencyIso { get; set; }
        public decimal? CreditLimit { get; set; }
        public decimal? DebitLimit { get; set; }
          public decimal? GLDebitLimit { get; set; }
          public decimal? GLCreditLimit { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Status { get; set; }
        public int? UserId { get; set; }
         public int? LoginUserId { get; set; }
    }

}