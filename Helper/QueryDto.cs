using System.Collections.Generic;
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;

namespace RevAssuranceApi.Helper
{
    public class QueryDto
    {
        public string query { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string UserId { get; set; }
        public string TableName { get; set; }

       

    }
}