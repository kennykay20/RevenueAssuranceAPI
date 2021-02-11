namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class GetUserDetailsParamCoreBnkingDTO
    {
         public string loginId { get; set; }

         public int? BankServiceItbId { get; set; }
          public int? ConnectionStringId { get; set; } = 1;
 
    }
}