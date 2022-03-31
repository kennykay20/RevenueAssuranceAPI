// using System;
// using System.Collections.Generic;
// using System.Data;
// using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using anythingGoodApi.AnythingGood.DATA.Models;
using RevAssuranceApi.AppSettings;
// using anythingGoodApi.Logger.Interface;

using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using RevAssuranceApi.TokenGen;
using System.Data;
using System.Data.SqlClient;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;

// using Microsoft.AspNetCore.Hosting;
// using Microsoft.AspNetCore.Mvc;
// using System.IO;
// using System.Net.Http.Headers;

namespace RevAssuranceApi.Controllers
{
      [Produces("application/json")]
      [ApiController]
      [Route("api/v1/[controller]")]
	  //[Authorize]

	public class UploadVehicleController : Controller
	{
		private IHostingEnvironment _hostingEnvironment;
		 TokenGenerator TokenGenerator;  
 		IDbConnection db = null;
		IConfiguration _configuration;
		 AppSettingsPath AppSettingsPath ;
		public UploadVehicleController(IHostingEnvironment hostingEnvironment, IConfiguration configuration)
		{
			_configuration = configuration;
			_hostingEnvironment = hostingEnvironment;
			TokenGenerator = new TokenGenerator(_configuration);
			 AppSettingsPath = new AppSettingsPath(_configuration);
			 db = new SqlConnection(AppSettingsPath.GetDefaultCon());
		}

		[HttpPost, DisableRequestSizeLimit]
		public async Task<ActionResult> UploadFile()
		{
			 string uploadBy = string.Empty, VehicleInfoId = string.Empty;
			 
			try
			{
				var files = Request.Form.Files;
				string folderName =  AppSettingsPath.Upload("vehicle"); // "Upload";
				string webRootPath = _hostingEnvironment.WebRootPath;
				string newPath = Path.Combine(webRootPath, folderName);

                
                 foreach(var t in Request.Form.Keys){

					 	var vehicle =  t.Split(":");
                        uploadBy =vehicle[1];
						VehicleInfoId = vehicle[3];

						uploadBy = RevAssuranceApi.Helper.UserEncrypt.Decrypt(uploadBy);

                 } 



				if (!Directory.Exists(newPath))
				{
					Directory.CreateDirectory(newPath);
				}
				if (files != null)
				{
				    foreach (var file in files)
					{
						string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
						string fullPath = Path.Combine(newPath, fileName);
						using (var stream = new FileStream(fullPath, FileMode.Create))
						{
							file.CopyTo(stream);

							DynamicParameters param = new DynamicParameters();

							var uploadVehicle = new UploadVehicle();

							param.Add("@VehicleInfoId", VehicleInfoId);
							param.Add("@ImagePath", fullPath);
							param.Add("@Status", 'A');

							var rtn = new DapperDATAImplementation<UploadVehicle>();
							
							var response = await rtn.ResponseObj("sp_UploadVehileFile", param, db);
					
							
						
						
						
						}
					}	
				
				}
			

				   var getToken = TokenGenerator.GetToken();
                   return Ok( new {
                            token = getToken.Token,
                            expiration = getToken.TokenExpireTime,
                            response = "Upload Successful.",
                            
                        }); 
			}
			catch (System.Exception ex)
			{
				return Json("Upload Failed: " + ex.Message);
			}
		}

		// [HttpPost, DisableRequestSizeLimit]
		// public ActionResult UploadFile()
		// {
		// 	try
		// 	{
		// 		var file = Request.Form.Files[0];
		// 		string folderName = "Upload";
		// 		string webRootPath = _hostingEnvironment.WebRootPath;
		// 		string newPath = Path.Combine(webRootPath, folderName);

        //          string uploadBy;
        //          foreach(var t in Request.Form.Keys){

        //                 uploadBy = t.Split(":")[1];
        //          } 



		// 		if (!Directory.Exists(newPath))
		// 		{
		// 			Directory.CreateDirectory(newPath);
		// 		}
		// 		if (file.Length > 0)
		// 		{
					
		// 			string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
		// 			string fullPath = Path.Combine(newPath, fileName);
		// 			using (var stream = new FileStream(fullPath, FileMode.Create))
		// 			{
		// 				file.CopyTo(stream);
		// 			}
		// 		}
		// 		return Json("Upload Successful.");
		// 	}
		// 	catch (System.Exception ex)
		// 	{
		// 		return Json("Upload Failed: " + ex.Message);
		// 	}
		// }

	}


}

   