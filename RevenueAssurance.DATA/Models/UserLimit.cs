using System.ComponentModel.DataAnnotations;

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    public class UserLimit
    {
        [Key]
        public string UserName { get; set; }
        public decimal DepDrLimit { get; set; }
        public decimal DepCrLimit { get; set; }
        public decimal GLDrLimit { get; set; }
        public decimal GLCrLimit { get; set; }
        public string Ccy { get; set; }
         public int? ConnectionStringId { get; set; } = 1;
 
    }
}