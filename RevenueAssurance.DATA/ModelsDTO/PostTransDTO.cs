using System;
using System.Collections.Generic;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class PostTransDTO
    {
        public CbsTransactionDTO CbsTransactionDTO { get; set;}
        public List<CbsTransactionDTO> ListCbsTransactionDTO { get; set;}
        public List<ChargesFormat> ChargesFormat { get; set;}
        public List<GetApproveServiceDTO> ListGetApproveServiceDTO {get; set;}
        public List<RejectionReasonDTO> ListRejectionReasonDTO { get; set;}
         public string LoginUserName { get; set;}
         public int UserId { get; set; }
         public Int64? ItbId { get; set; }
         public int? LoginUserId { get; set; }
         public string TransactionDate {get;set;}
    }
}