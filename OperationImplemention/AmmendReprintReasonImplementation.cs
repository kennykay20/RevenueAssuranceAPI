using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using anythingGoodApi.AnythingGood.DATA.Models;
using RevAssuranceApi.AppSettings;

using RevAssuranceApi.TokenGen;
using Dapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RevAssuranceApi.Helper;
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;
using anythingGoodApi.AnythingGood.DATA;
using RevAssuranceApi.OperationImplemention;
using RevAssuranceApi.WebServices;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using RevAssuranceApi.RevenueAssurance.DATA.Models;

namespace RevAssuranceApi.OperationImplemention
{
    
    public class AmmendReprintReasonImplementation
    {
        IRepository<admAmendReprintReason> _repoadmAmmendAndRepriReason;
        Formatter _Formatter = new Formatter();
        IConfiguration _configuration;
        public AmmendReprintReasonImplementation( 
                                          IConfiguration configuration,
                                          IRepository<admAmendReprintReason> 
                                          repoadmAmmendAndRepriReason
                                          )
        {
            _repoadmAmmendAndRepriReason = repoadmAmmendAndRepriReason;
            _configuration = configuration;
        }
        public async Task<List<admAmendReprintReason>> GetAllReasons()
        {
            var get = await _repoadmAmmendAndRepriReason.GetManyAsync(c=> c.ReasonId > 0);

            return get.ToList();
        }

        public string replacePrintingAPG(string txt, OprInstrument oprInstrument){
           
            try
            {
                 
                 string contain1 = "(Name and Address of Beneficiary)";
                 string contain2 = "{{RefNo}}";
                 string contain3 = "{{DateIssued}}";
                 string contain4 = "(NAME OF CUSTOMER/CONTRACTOR)";
                 string contain5 = "(NAME OF BENEFICIARY)";
                 string contain6 = "(Amount in Figures)";
                 string contain7 = "(Amount in words)";

                 string NameAndAddress = $"{ oprInstrument.AcctName} and {oprInstrument.AdresseeName}";
                 if(txt.Contains(contain1)) {
                    string Txt  = txt.Replace(contain1, NameAndAddress);
                    txt = Txt;
                 }

                if(txt.Contains(contain2)) {
                      string Txt  = txt.Replace(contain2, oprInstrument.ReferenceNo);
                      txt = Txt;
                }

                 if(txt.Contains(contain3)) {
                     string date = DateTime.Now.ToString("dddd, dd MMMM yyyy");
                     txt  = txt.Replace(contain3, date);
                }

                if(txt.Contains(contain4)) {
                     string Txt = txt.Replace(contain4, oprInstrument.AcctName);
                     txt  = Txt;
                }

                if(txt.Contains(contain5)) {
                     string Txt = txt.Replace(contain5, oprInstrument.Beneficiary);
                     txt  = Txt;
                }
                if(txt.Contains(contain6)) {
                      if(oprInstrument.Amount == null){
                       oprInstrument.Amount = 0;
                   }
                    string Txt = txt.Replace(contain6, _Formatter.FormattedAmount((decimal)oprInstrument.Amount));
                     txt  = Txt;
                }

                  if(txt.Contains(contain7)) {
                       if(oprInstrument.Amount == null){
                       oprInstrument.Amount = 0;
                   }
                    string AmtInWords = AmountInWords.NumberToWords((Int64)oprInstrument.Amount);
                    string Txt = txt.Replace(contain7, AmtInWords);
                     txt  = Txt;
                }

                return txt;

            }
            catch(Exception ex){
                var exM = ex == null ? ex.InnerException.Message : ex.Message;
                 LogManager _LogManager = new LogManager(_configuration);
                _LogManager.SaveLog($"Error replacePrintingAPG AmmenReprintIml: { exM } ");

                return txt;
            }
           
        }

        public string replacePrintingBidSecuirty(string txt, OprInstrument oprInstrument){
            try
            {
                 
                 string contain1 = "{{BeneficiaryInfo}}";
                 string contain2 = "{{RefNo}}";
                 string contain3 = "(NAME OF CUSTOMER/TENDERER)";
                 string contain4 = "(Amount in Figures)";
                 string contain5 = "(Amount in words)";
                 string contain6 = "{{DateIssued}}";
                 
                 string BeneficiaryInfo = $"{ oprInstrument.Beneficiary }";
                 if(txt.Contains(contain1)) {
                     string Txt = txt.Replace(contain1, BeneficiaryInfo);
                     txt  = Txt;
                 }

                if(txt.Contains(contain2)) {
                     string Txt  = txt.Replace(contain2, oprInstrument.ReferenceNo);
                      txt  = Txt;
                }

                if(txt.Contains(contain3)) {
                     string Txt  = txt.Replace(contain3, oprInstrument.AcctName);
                      txt  = Txt;
                }
                if(txt.Contains(contain4)) {
                   if(oprInstrument.Amount == null){
                       oprInstrument.Amount = 0;
                   }
                     string Txt   = txt.Replace(contain4, _Formatter.FormattedAmount((decimal)oprInstrument.Amount));
                      txt  = Txt;
                }

                if(txt.Contains(contain5)) {
                    if(oprInstrument.Amount == null){
                       oprInstrument.Amount = 0;
                   }
                    string AmtInWords = AmountInWords.NumberToWords((Int64)oprInstrument.Amount);
                     string Txt   = txt.Replace(contain5, AmtInWords);
                      txt  = Txt;
                }

                 if(txt.Contains(contain6)) {
                      string date = DateTime.Now.ToString("dddd, dd MMMM yyyy");
                      string Txt   = txt.Replace(contain6, date);
                       txt  = Txt;
                }

                return txt;

            }
            catch(Exception ex){
                var exM = ex == null ? ex.InnerException.Message : ex.Message;
                 LogManager _LogManager = new LogManager(_configuration);
                _LogManager.SaveLog($"Error replacePrintingBidSecuirty AmmenReprintIml: { exM } ");
               
            }
           return txt;
        }

        public string replacePrintingCustomBond(string txt, OprInstrument oprInstrument){
            try
            {
                 
                 string contain1 = "{{BeneficiaryInfo}}";
                 string contain2 = "{{RefNo}}";
                 string contain3 = "(NAME OF CUSTOMER/TENDERER)";
                 string contain4 = "(Amount in Figures)";
                 string contain5 = "(Amount in words)";
                 string contain6 = "{{DateIssued}}";
                 
                 string BeneficiaryInfo = $"{ oprInstrument.Beneficiary }";
                 if(txt.Contains(contain1)) {
                     string Txt = txt.Replace(contain1, BeneficiaryInfo);
                     txt  = Txt;
                 }

                if(txt.Contains(contain2)) {
                     string Txt  = txt.Replace(contain2, oprInstrument.ReferenceNo);
                      txt  = Txt;
                }

                if(txt.Contains(contain3)) {
                     string Txt  = txt.Replace(contain3, oprInstrument.AcctName);
                      txt  = Txt;
                }
                if(txt.Contains(contain4)) {
                   if(oprInstrument.Amount == null){
                       oprInstrument.Amount = 0;
                   }
                     string Txt   = txt.Replace(contain4, _Formatter.FormattedAmount((decimal)oprInstrument.Amount));
                      txt  = Txt;
                }

                if(txt.Contains(contain5)) {
                    if(oprInstrument.Amount == null){
                       oprInstrument.Amount = 0;
                   }
                    string AmtInWords = AmountInWords.NumberToWords((Int64)oprInstrument.Amount);
                     string Txt   = txt.Replace(contain5, AmtInWords);
                      txt  = Txt;
                }
                
                 if(txt.Contains(contain6)) {
                      string date = DateTime.Now.ToString("dddd, dd MMMM yyyy");
                      string Txt   = txt.Replace(contain6, date);
                       txt  = Txt;
                }

                return txt;

            }
            catch(Exception ex){
                var exM = ex == null ? ex.InnerException.Message : ex.Message;
                 LogManager _LogManager = new LogManager(_configuration);
                _LogManager.SaveLog($"Error replacePrintingBidSecuirty AmmenReprintIml: { exM } ");
               
            }
           return txt;
        }

        public string replacePrintingPerformanceBond(string txt, OprInstrument oprInstrument){
            try
            {
                 
                 string contain1 = "{{BeneficiaryInfo}}";
                 string contain2 = "{{RefNo}}";
                 string contain3 = "(NAME OF CUSTOMER/TENDERER)";
                 string contain4 = "(Amount in Figures)";
                 string contain5 = "(Amount in words)";
                 string contain6 = "{{DateIssued}}";
                 
                 string BeneficiaryInfo = $"{ oprInstrument.Beneficiary }";
                 if(txt.Contains(contain1)) {
                     string Txt = txt.Replace(contain1, BeneficiaryInfo);
                     txt  = Txt;
                 }

                if(txt.Contains(contain2)) {
                     string Txt  = txt.Replace(contain2, oprInstrument.ReferenceNo);
                      txt  = Txt;
                }

                if(txt.Contains(contain3)) {
                     string Txt  = txt.Replace(contain3, oprInstrument.AcctName);
                      txt  = Txt;
                }
                if(txt.Contains(contain4)) {
                   if(oprInstrument.Amount == null){
                       oprInstrument.Amount = 0;
                   }
                     string Txt   = txt.Replace(contain4, _Formatter.FormattedAmount((decimal)oprInstrument.Amount));
                      txt  = Txt;
                }

                if(txt.Contains(contain5)) {
                    if(oprInstrument.Amount == null){
                       oprInstrument.Amount = 0;
                   }
                    string AmtInWords = AmountInWords.NumberToWords((Int64)oprInstrument.Amount);
                     string Txt   = txt.Replace(contain5, AmtInWords);
                      txt  = Txt;
                }
                
                 if(txt.Contains(contain6)) {
                      string date = DateTime.Now.ToString("dddd, dd MMMM yyyy");
                      string Txt   = txt.Replace(contain6, date);
                       txt  = Txt;
                }

                return txt;

            }
            catch(Exception ex){
                var exM = ex == null ? ex.InnerException.Message : ex.Message;
                 LogManager _LogManager = new LogManager(_configuration);
                _LogManager.SaveLog($"Error replacePrintingBidSecuirty AmmenReprintIml: { exM } ");
               
            }
           return txt;
        }
  
        public string retentionplacePrintingRetententionBond(string txt, OprInstrument oprInstrument){
            try
            {
                 
                 string contain1 = "{{BeneficiaryInfo}}";
                 string contain2 = "{{RefNo}}";
                 string contain3 = "(NAME OF CUSTOMER/TENDERER)";
                 string contain4 = "(Amount in Figures)";
                 string contain5 = "(Amount in words)";
                 string contain6 = "{{DateIssued}}";
                 
                 string BeneficiaryInfo = $"{ oprInstrument.Beneficiary }";
                 if(txt.Contains(contain1)) {
                     string Txt = txt.Replace(contain1, BeneficiaryInfo);
                     txt  = Txt;
                 }

                if(txt.Contains(contain2)) {
                     string Txt  = txt.Replace(contain2, oprInstrument.ReferenceNo);
                      txt  = Txt;
                }

                if(txt.Contains(contain3)) {
                     string Txt  = txt.Replace(contain3, oprInstrument.AcctName);
                      txt  = Txt;
                }
                if(txt.Contains(contain4)) {
                   if(oprInstrument.Amount == null){
                       oprInstrument.Amount = 0;
                   }
                     string Txt   = txt.Replace(contain4, _Formatter.FormattedAmount((decimal)oprInstrument.Amount));
                      txt  = Txt;
                }

                if(txt.Contains(contain5)) {
                    if(oprInstrument.Amount == null){
                       oprInstrument.Amount = 0;
                   }
                    string AmtInWords = AmountInWords.NumberToWords((Int64)oprInstrument.Amount);
                     string Txt   = txt.Replace(contain5, AmtInWords);
                      txt  = Txt;
                }
                
                 if(txt.Contains(contain6)) {
                      string date = DateTime.Now.ToString("dddd, dd MMMM yyyy");
                      string Txt   = txt.Replace(contain6, date);
                       txt  = Txt;
                }

                return txt;

            }
            catch(Exception ex){
                var exM = ex == null ? ex.InnerException.Message : ex.Message;
                 LogManager _LogManager = new LogManager(_configuration);
                _LogManager.SaveLog($"Error replacePrintingBidSecuirty AmmenReprintIml: { exM } ");
               
            }
           return txt;
        }

    }
}