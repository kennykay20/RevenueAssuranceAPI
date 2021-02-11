using System.Collections.Generic;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class BatchControlDTO
    {
          public BatchControl  BatchControl { get; set;}

          public BatchControlTemp  BatchControlTemp { get; set;}
          public List<BatchControl>  ListBatchControl { get; set;}

          public List<BatchItemsTemp> ListBatchItemsTemp { get; set;}
          public List<BatchItems> ListBatchItems { get; set;}
          public int? LoginUserId { get; set; }
          public BatchItems BatchItems { get; set;}
          public BatchItemsTemp BatchItemsTemp {get; set;}

          public string AcctNo {get; set;}
         public string AcctType {get; set;}
          public string CcyCode {get; set;}
           public string LoginUserName {get; set;}
           public string TransAmout  {get; set;}
           public int? ServiceId  {get; set;}
           public int? OriginBranchNo {get; set;}
            public int? ProcessingDept {get; set;}
            public int? Dept {get; set;}
		 public List<oprServiceCharges> ListoprServiceCharge { get; set;}

         public bool ChargeThisAcct  { get; set;}

         public string TransactionDate { get; set;} 
         public string ValueDate { get; set;}
         public long? BatchNo { get; set;}

         public List<UploadBulkFilesValidator> UploadBulkFilesValidatorList   { get; set;}
    }
}