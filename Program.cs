using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotNetCoreSqlDb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.AddAzureWebAppDiagnostics();
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();

                    // Override ASPNETCORE_URLS, as this can be set to http://+:8081 in error when deployed as App Service Linux container and Managed Service Identity enabled
                    string urlSpec = Environment.GetEnvironmentVariable("APPSETTING_ASPNETCORE_URLS");
                    if (!String.IsNullOrEmpty(urlSpec))
                        webBuilder.UseUrls(urlSpec);
                });
    }
}
