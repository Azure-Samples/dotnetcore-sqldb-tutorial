using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using DotNetCoreSqlDb.Models;
using System.Collections;

namespace DotNetCoreSqlDb
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
            services.AddControllersWithViews();

            Console.WriteLine();
            Console.WriteLine("Environment variables: ");


            foreach (DictionaryEntry envVar in Environment.GetEnvironmentVariables())
                Console.WriteLine(" {0} : {1}", envVar.Key, envVar.Value);

            string aspEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (String.IsNullOrEmpty(aspEnvironment))
                throw new InvalidOperationException("Environment variable ASPNETCORE_ENVIRONMENT should be set, aborting");

            switch (aspEnvironment)
            {
                case "Development":
                case "Offline":
                    services.AddDbContext<MyDatabaseContext>(options =>
                            options.UseSqlite("Data Source=localdatabase.db"));
                    break;

                default:
                    string connectionString = Configuration.GetConnectionString("MyDbConnection");
                    if (String.IsNullOrEmpty(connectionString))
                        throw new InvalidOperationException("Connection string MyDbConnection should be set, aborting");
                    services.AddDbContext<MyDatabaseContext>(options =>
                            options.UseSqlServer(connectionString));
                    break;
            }

            // Automatically perform database migration
            //services.BuildServiceProvider().GetService<MyDatabaseContext>().Database.Migrate();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Todos}/{action=Index}/{id?}");
            });
        }
    }
}
