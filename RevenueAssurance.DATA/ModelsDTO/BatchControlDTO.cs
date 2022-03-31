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
          public List<BatchParamsItemTempDTO> BatchParamsItemTempDTOs { get; set; }
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

    public class BatchParamsItemTempDTO
    {
        public long ItbId						{get; set; }
            public int? ServiceId					{get; set; }
            public long BatchNo				{get; set; }
            public string BranchNo					{get; set; }
            public string AcctNo					{get; set; }
            public string AcctType					{get; set; }
            public string CbsTC						{get; set; }
            public string Narration					{get; set; }
            public string CcyCode					{get; set; }
            public decimal? Amount					{get; set; }
            public string DrCr						{get; set; }
            public string ChargeCode				{get; set; }
            public decimal? ChargeAmount				{get; set; }
            public string ChgNarration				{get; set; }
            public System.DateTime? TransactionDate			{get; set; }
            public string ValueDate					{get; set; }
            public System.DateTime? DateCreated				{get; set; }
            public string ServiceStatus				{get; set; }
            public int? DeptId					{get; set; }
            public  int? ProcessingDept			{get; set; }
            public int BatchSeqNo				{get; set; }
            public  int? UserId					{get; set; }
            public string Direction					{get; set; }
            public string OriginatingBranchId		{get; set; }
            public string ChequeNo		{get; set; } 
             public string ReferenceNo		{get; set; } 
             public string ValErrorIds { get; set; }
    }

}