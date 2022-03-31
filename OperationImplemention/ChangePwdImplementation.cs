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
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using RevAssuranceApi.WebServices;
using RevAssuranceApi.Response;

namespace RevAssuranceApi.OperationImplemention
{
    public class ChangePwdImplementation
    {
        IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        AppSettingsPath AppSettingsPath ;
         IDbConnection db = null;
        DeserializeSerialize<LoginReturnProperty> _DeserializeSerialize;
        IRepository<admLicenseSetUp> _repositoryLicense;
        IRepository<admClientProfile> _repoClientProfile;
        IRepository<admUserProfile> _repoadmUserProfile;
        IRepository<admRole> _repoadmRole;
        IRepository<admBankBranch> _repoadmBankBranch;
        ApplicationDbContext _ApplicationDbContext;
        IRepository<admUserLogin> _repoadmUserLogin;
        HeaderLogin _HeaderLogin; 
        WebServiceCaller _clientcaller;
        LogManager  _LogManager;
        public ChangePwdImplementation( 
                                          IConfiguration configuration,
                                          DeserializeSerialize<LoginReturnProperty> DeserializeSerialize,
                                          IRepository<admLicenseSetUp> repositoryLicense,
                                          IRepository<admClientProfile> repoClientProfile,
                                          IRepository<admUserProfile> repoadmUserProfile,
                                          ApplicationDbContext ApplicationDbContext,
                                          HeaderLogin HeaderLogin,
                                          WebServiceCaller clientcaller,
                                          IRepository<admUserLogin> repoadmUserLogin,
                                          IRepository<admRole> repoadmRole,
                                          IRepository<admBankBranch> repoadmBankBranch,
                                          LogManager  LogManager
                                          )
        {

            _configuration = configuration;
            _DeserializeSerialize = DeserializeSerialize;
            _repositoryLicense = repositoryLicense;
            _repoClientProfile = repoClientProfile;
            _repoadmUserProfile = repoadmUserProfile;
            _ApplicationDbContext = ApplicationDbContext;
            _HeaderLogin = HeaderLogin;
            _clientcaller = clientcaller;
            _repoadmUserLogin = repoadmUserLogin;
             AppSettingsPath = new AppSettingsPath(_configuration);
             db = new SqlConnection(AppSettingsPath.GetDefaultCon());
             _repoadmRole = repoadmRole;
             _repoadmBankBranch = repoadmBankBranch;
             _LogManager = LogManager;
        }

        public async Task<ApiResponse> ChangePasswordOld(ChangePwdDTO ChangePwdDTO)
        {
             if(ChangePwdDTO.OldPassword == ChangePwdDTO.NewPassword)
               {
                 ApiResponse.ResponseMessage = "Old Password and New Password Couldn't be the same";
                 return ApiResponse;
               
               }
            var getUser = await _repoadmUserProfile.GetAsync(c=> c.LoginId == ChangePwdDTO.LoginId);
            if(getUser != null)
            {
               string encPws = Cryptors.EncryptNoKey(ChangePwdDTO.OldPassword);

               if(encPws == getUser.Password)
               {
                    string encPwsNew = Cryptors.EncryptNoKey(ChangePwdDTO.NewPassword);

                    getUser.Password = encPwsNew;
                    _repoadmUserProfile.Update(getUser);

                    int Save = await _ApplicationDbContext.SaveChanges(getUser.UserId);

                    if(Save > 0)
                    {
                        ApiResponse.ResponseCode = 0;
                        ApiResponse.ResponseMessage = "Password Changed Successfully!";
                    }
               }
               else
               {
                       
                        ApiResponse.ResponseMessage = "InCorrect Password!";
               }
            }
            return ApiResponse;
        }

        public async Task<ApiResponse> ChangePassword(ChangePwdDTO ChangePwdDTO)
        {
            if(ChangePwdDTO.OldPassword == ChangePwdDTO.NewPassword)
            {
                ApiResponse.ResponseMessage = "Old Password and New Password Couldn't be the same";
                return ApiResponse;
               
            }
            var getUser = await _repoadmUserProfile.GetAsync(c=> c.LoginId == ChangePwdDTO.LoginId);

            var clientProfile = await _repoClientProfile.GetAsync(null);
            if(getUser != null)
            {
                if(ChangePwdDTO.NewPassword.Count() <= clientProfile.MaxiPasswordLength)
                {
                    if(clientProfile.ComplexPassword == "C")
                    {
                        if (ChangePwdDTO.NewPassword.Count(char.IsUpper) >= clientProfile.Uppercase && ChangePwdDTO.NewPassword.Count(char.IsDigit) >= clientProfile.NumericNumber && ChangePwdDTO.NewPassword.Count(c => !char.IsLetterOrDigit(c)) >= clientProfile.SpecialCharacter)
                        {
                                    
                            string encOldPws = Cryptors.EncryptNoKey(ChangePwdDTO.OldPassword);

                            if(encOldPws == getUser.Password)
                            {
                                var encryptNewPass = Cryptors.EncryptNoKey(ChangePwdDTO.NewPassword);

                                if(getUser.EnforcePswdChange == "Y"){
                                    getUser.EnforcePswdChange = "N";
                                }
                                getUser.Password = encryptNewPass;
                                
                                await _ApplicationDbContext.SaveChanges(getUser.UserId);

                                _repoadmUserProfile.Update(getUser);

                                int Save = await _ApplicationDbContext.SaveChanges(getUser.UserId);

                                if(Save > 0)
                                {
                                    ApiResponse.ResponseCode = 0;
                                    ApiResponse.ResponseMessage = "Password Changed Successfully!";
                                }

                            }
                            else{
                                ApiResponse.ResponseMessage = "InCorrect Password!";
                            }
                        }
                        else
                        {
                            ApiResponse.ResponseCode = -5;
                        }
                    }
                    else{

                        var encryptNewPass = Cryptors.EncryptNoKey(ChangePwdDTO.NewPassword);
                                
                        if(getUser.EnforcePswdChange == "Y"){
                            getUser.EnforcePswdChange = "N";
                        }
                        
                        getUser.Password = encryptNewPass;
                                
                        await _ApplicationDbContext.SaveChanges(getUser.UserId);

                        _repoadmUserProfile.Update(getUser);

                        int Save = await _ApplicationDbContext.SaveChanges(getUser.UserId);

                        if(Save > 0)
                        {
                            ApiResponse.ResponseCode = 0;
                            ApiResponse.ResponseMessage = "Password Changed Successfully!";
                        }
                        
                    }
                }
                else{
                    
                    ApiResponse.ResponseCode = -1;
                    ApiResponse.ResponseMessage = "New Password is greater than Maximum Length of Set Password";
                }
                
            }
            return ApiResponse;
        }
    }


}