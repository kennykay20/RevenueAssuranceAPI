using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using RevAssuranceApi.AppSettings;
using Dapper;
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;


using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using anythingGoodApi.AnythingGood.DATA;
using anythingGoodApi.AnythingGood.DATA.Models;
using RevAssuranceApi.TokenGen;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RevAssuranceApi.OperationImplemention;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;

namespace RevAssuranceApi.OperationImplemention
{
    public class ComputeChargesImplementation
    {
       
        AppSettingsPath AppSettingsPath ;
         IDbConnection db = null;
            IConfiguration _configuration;
        Formatter _Formatter = new Formatter();
        
        public ComputeChargesImplementation(
            IConfiguration configuration){
             _configuration = configuration;
         AppSettingsPath = new AppSettingsPath(_configuration); 
           db = new SqlConnection(AppSettingsPath.GetDefaultCon());

        } 
        public async Task<RevCalChargeModel> CalChargeModel(OperationViewModel OperationViewModel, 
                                                            string ChgAcctBr, string AcctType, 
                                                            string AcctCcy, string Ammendment =  "N", 
                                                            string Reprint = "N", decimal IChgAmount = 0)
        {
            IDapperDATA<RevCalChargeModel> rtn = new DapperDATAImplementation<RevCalChargeModel>();
            DynamicParameters param = new DynamicParameters();

            /*LogManager.SaveLog($"RevCalChargeModel param OperationViewModel.serviceId: { OperationViewModel.serviceId } OperationViewModel.InstrumentAmount: {OperationViewModel.InstrumentAmount} OperationViewModel.InstrumentAcctNoIni: {OperationViewModel.InstrumentAcctNoIni} OperationViewModel.OprAPG.AcctType: {OperationViewModel.OprAPG.AcctType} OperationViewModel.OprAPG.AcctCcy: {OperationViewModel.OprAPG.AcctCcy} OperationViewModel.TempTypeIni {OperationViewModel.TempTypeIni} ChgAcctBr: {ChgAcctBr} OperationViewModel.OprAPG.ChargeAcctCcy: {OperationViewModel.OprAPG.ChargeAcctCcy}");
            */
            param.Add("@pnServiceId", OperationViewModel.serviceId);
            param.Add("@pnDirection", 1);
            param.Add("@pnTranAmount",  OperationViewModel.TransAmount);
            param.Add("@pnProductCode", null);
            param.Add("@PsAcctNo", OperationViewModel.InstrumentAcctNo);
            param.Add("@psAcctType", AcctType);
            param.Add("@psCustomerNo", null);
            param.Add("@psCurrency", AcctCcy);
            param.Add("@pnTemplateId", OperationViewModel.TempTypeId);
            param.Add("@pnTransCount", 1);
            param.Add("@psDrCr", "DR");
            param.Add("@psAmmendment", Ammendment);
            param.Add("@psReprint", Reprint);
            param.Add("@pnIChgAmount", IChgAmount);
            param.Add("@pnChgAcctBr", ChgAcctBr);
            param.Add("@psChgActCcy", OperationViewModel.InstrumentCcy);
            param.Add("@pnChargeCode", OperationViewModel.ChargeCode);

            if(!string.IsNullOrWhiteSpace(OperationViewModel.pnCalcAmt))
              param.Add("@pnCalcAmt", OperationViewModel.pnCalcAmt);
           

            var data = await rtn.ResponseObj("Isp_RevCalcCharge", param, db);

            return data;
        }

        public async Task<RevCalChargeModel> GenServiceRef(int? ServiceId)
        {
            IDapperDATA<RevCalChargeModel> rtn = new DapperDATAImplementation<RevCalChargeModel>();
            DynamicParameters param = new DynamicParameters();

            param.Add("@pnServiceId", ServiceId);

            var getTransRef = await rtn.ResponseObj("Isp_GetServiceRef", param, db);
            return getTransRef;
        }

        public async Task<RevCalChargeModel> GenTranRef(int? ServiceId)
        {
            try
            {
                IDapperDATA<RevCalChargeModel> rtn = new DapperDATAImplementation<RevCalChargeModel>();
                 
                DynamicParameters param = new DynamicParameters();

                param.Add("@pnServiceId", ServiceId);
                // param.Add("@rsTransRef", DbType.String, direction: ParameterDirection.Output);
                // param.Add("@rsChannel", DbType.String, direction: ParameterDirection.Output);
                // param.Add("@rnErrorCode", DbType.Int32, direction: ParameterDirection.Output);
                // param.Add("@rsErrorMsg", DbType.String, direction: ParameterDirection.Output);
               

                var getTransRef = await rtn.ResponseObj("isp_GetTransRef2", param, db);
                return getTransRef;
            }
            catch(Exception ex)
            {
                var exM = ex;
                return null;
            }
            
        }
    
        public DateTime? GetNextRunDate(DateTime? StartDate, string TimeBasis)
        {
            //var rtv = new ReturnValues();

            try
            {
                if (StartDate != null && !string.IsNullOrEmpty(TimeBasis))
                {
                    if (TimeBasis == "DAYS")
                    {
                        return StartDate.Value.AddDays(1);
                    }
                    else if (TimeBasis == "WEEKS")
                    {
                        return StartDate.Value.AddDays(7);
                    }
                    else if (TimeBasis == "MONTHS")
                    {
                        return StartDate.Value.AddMonths(1);
                    }
                    else if (TimeBasis == "YEARS")
                    {
                        return StartDate.Value.AddYears(1);
                    }

                }

            }
            catch (Exception ex)
            {
              //  LogManager.SaveLog(ex.Message == null ? ex.InnerException.ToString() : ex.Message.ToString());

            }

            return (DateTime?)null;
        }

        public DateTime? GetEndRunDate(DateTime? StartDate, string TimeBasis, int noOfInstallment,string nextInstDate,int Tenur)
        {
            var rtv = new ReturnValues();

            try
            {


                if (StartDate != null && !string.IsNullOrEmpty(TimeBasis) && !string.IsNullOrEmpty(nextInstDate))
                {
                    if (TimeBasis == "DAYS")
                    {
                        return Convert.ToDateTime(StartDate).AddDays(Tenur - 1);
                    }
                    
                    else if (TimeBasis == "MONTHS")
                    {
                        return Convert.ToDateTime(StartDate).AddMonths(noOfInstallment - 1);
                    }
                    else if (TimeBasis == "YEARS")
                    {
                        return Convert.ToDateTime(StartDate).AddYears(noOfInstallment - 1);
                    }

                }

            }
            catch (Exception ex)
            {
                //LogManager.SaveLog(ex.Message == null ? ex.InnerException.ToString() : ex.Message.ToString());

            }

            return (DateTime?)null;
        }

    
    
    }
}