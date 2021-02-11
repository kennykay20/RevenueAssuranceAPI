using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace RevAssuranceApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                 //.UseSerilog((hosti   ngContext, loggerConfiguration) =>
                  //  loggerConfiguration
                   // .ReadFrom.Configuration(hostingContext.Configuration)
                    //.Enrich.FromLogContext())
                .UseUrls("http://localhost:6009");
    }
}
