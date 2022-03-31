namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
         public class MiniStatementResp
        {
            public string acct_no { get; set; }
            public string acct_type { get; set; }
            public string TransDate { get; set; }
            public string ValueDate { get; set; }
            public string Narration { get; set; }
            public string Debit { get; set; }
            public string Credit { get; set; }
            public string balance { get; set; }
            public string iso_code { get; set; }
            public string ChangeFlag { get; set; }
            public string UnclearedFalg { get; set; }
        }

}