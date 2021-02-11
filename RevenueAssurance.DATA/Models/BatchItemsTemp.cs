using System;
using System.ComponentModel.DataAnnotations;

namespace RevAssuranceApi.RevenueAssurance.DATA.Models
{
    public class BatchItemsTemp
    {
            [Key]
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
            public DateTime? TransactionDate			{get; set; }
            public DateTime? ValueDate					{get; set; }
            public DateTime? DateCreated				{get; set; }
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