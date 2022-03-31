using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using RevAssuranceApi.OperationImplemention;
using RevAssuranceApi.Helper;
using RevAssuranceApi.WebServices;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.Query;
using RevAssuranceApi.RevenueAssurance.Services.ServiceImplementation;
using RevAssuranceApi.RevenueAssurance.Repository.Implementation;
using RevAssuranceApi.RevenueAssurance.Services.ServiceInterface;
using RevAssuranceApi.RevenueAssurance.Repository.Command;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using AutoMapper;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;
using RevAssuranceApi.AppSettings;
using System.Data.SqlClient;

namespace RevAssuranceApi
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //  services.AddAutoMapper(x=> x.AddProfile(new MappingsProfile()));
            services.AddMvc();
            services.AddAutoMapper();

            /* Jwt Authentication start here */

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["Jwt:Site"],
                    ValidIssuer = Configuration["Jwt:Site"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SigningKey"]))
                };
            });

            /* Jwt Authentication Ends here */

            services.AddSingleton<IConfiguration>(Configuration);
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient(typeof(IUtilities), typeof(Utilities));
            services.AddTransient(typeof(IQueryUnitOfWork), typeof(QueryUnitOfWork));
            services.AddTransient(typeof(IQueryRepository<>), typeof(QueryRepository<>));
            services.AddTransient(typeof(ICommandUnitOfWork), typeof(CommandUnitOfWork));
            services.AddTransient(typeof(IQueryUnitOfWork), typeof(QueryUnitOfWork));

            services.AddScoped(typeof(IAuditTrailService), typeof(AuditTrailService));

            services.AddScoped(typeof(LoginImplementation), typeof(LoginImplementation));
            services.AddScoped(typeof(DapperDATAImplementation<>), typeof(DapperDATAImplementation<>));
            services.AddScoped(typeof(HeaderLogin), typeof(HeaderLogin));
            services.AddScoped(typeof(DeserializeSerialize<>), typeof(DeserializeSerialize<>));
            services.AddScoped(typeof(WebServiceCaller), typeof(WebServiceCaller));
            services.AddScoped(typeof(WebServiceClient), typeof(WebServiceClient));
            services.AddScoped(typeof(ChargeImplementation), typeof(ChargeImplementation));
            services.AddScoped(typeof(ServiceChargeImplementation), typeof(ServiceChargeImplementation));
            services.AddScoped(typeof(DocumentRetImplementation), typeof(DocumentRetImplementation));
            services.AddScoped(typeof(AccountValidationImplementation), typeof(AccountValidationImplementation));
            services.AddScoped(typeof(ApplicationReturnMessageImplementation), typeof(ApplicationReturnMessageImplementation));
            services.AddScoped(typeof(ComputeChargesImplementation), typeof(ComputeChargesImplementation));
            services.AddScoped(typeof(UsersImplementation), typeof(UsersImplementation));
            services.AddScoped(typeof(RoleAssignImplementation), typeof(RoleAssignImplementation));
            services.AddScoped(typeof(RoleAssignImplementation), typeof(RoleAssignImplementation));
            services.AddScoped(typeof(ColCollateralTypeImplementation), typeof(ColCollateralTypeImplementation));
            services.AddScoped(typeof(ApproveImplementation), typeof(ApproveImplementation));
            services.AddScoped(typeof(CBSTransImplementation), typeof(CBSTransImplementation));
            services.AddScoped(typeof(CoreBankingImplementation), typeof(CoreBankingImplementation));
            services.AddScoped(typeof(ApprovalValidation), typeof(ApprovalValidation));
            services.AddScoped(typeof(ApplicationReturnMessage), typeof(ApplicationReturnMessage));
            services.AddScoped(typeof(LogManager), typeof(LogManager));
            services.AddScoped(typeof(Formatter), typeof(Formatter));
            services.AddScoped(typeof(TransactionLogger), typeof(TransactionLogger));
            services.AddScoped(typeof(ChangePwdImplementation), typeof(ChangePwdImplementation));
            services.AddScoped(typeof(AcctStatementImplementation), typeof(AcctStatementImplementation));
            services.AddScoped(typeof(MandateImplementation), typeof(MandateImplementation));
            services.AddScoped(typeof(AmmendReprintReasonImplementation), typeof(AmmendReprintReasonImplementation));
            services.AddScoped(typeof(FunctionApiSetUpImplementation), typeof(FunctionApiSetUpImplementation));
            services.AddScoped(typeof(ImagesPathSettings), typeof(ImagesPathSettings));
            services.AddScoped(typeof(EmailImplementation), typeof(EmailImplementation));

            #region Dependencies

            #endregion



            LogManager _LogManager = new LogManager(Configuration);
            try
            {
                AppSettingsPath DBConSetUp = new AppSettingsPath(Configuration);

                bool UseEncryptedDBCon = DBConSetUp.UseEncryptedDBCon();
                string conString = DBConSetUp.GetDefaultCon();

                services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(conString));
            }
            catch (SqlException ex)
            {
                decimal number = ex.Number;
                var exM = ex == null ? ex.InnerException.Message : ex.Message;
            }

            services.AddMvc()
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    // Version = "v1",
                    Title = "Revenue Assurance Web API ",
                    // Description = "This manges Voyage Travels",
                    // TermsOfService = "None",
                    // Contact = new Contact() { Name = "Voyage Travels", Email = "VoyageTravels@gmail.com.ng", Url = "www.VoyageTravels.com.ng" }
                });
            });


            services.AddDistributedRedisCache(option =>
            {
                option.Configuration = Configuration.GetValue<string>("redis:host");
                option.InstanceName = "";
            });


            services.AddCors(options =>
             {
                 options.AddPolicy("CorsPolicy",
                       builder => builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials()
                   .Build());
             });


            // services.AddCors(options =>
            // {
            //     options.AddPolicy("CorsPolicy",
            //     builder =>
            //     {
            //         builder.WithOrigins()
            //                 .AllowAnyHeader()
            //                 .AllowAnyMethod();
            //     });
            // });



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //Register Token authentication below
            app.UseAuthentication();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // else
            // {
            //     app.UseHsts();
            // }


            app.UseMvc();
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value);
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
                c.RoutePrefix = string.Empty;
            });

            app.UseWelcomePage();

            app.UseCors("CorsPolicy");
        }
    }
}