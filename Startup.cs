﻿using System;
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
using Microsoft.EntityFrameworkCore;
using DotNetCoreSqlDb.Models;

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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // services.AddDbContext<MyDatabaseContext>(options =>
            //         options.UseSqlite("Data Source=localdatabase.db"));
            // Use SQL Database if in Azure, otherwise, use SQLite


            if(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
               services.AddDbContext<MyDatabaseContext>(options =>
                       options.UseSqlServer(Configuration.GetConnectionString("MyDbConnection")));
            else
               services.AddDbContext<MyDatabaseContext>(options =>
                       options.UseSqlite("Data Source=localdatabase.db"));

            // Automatically perform database migration
            services.BuildServiceProvider().GetService<MyDatabaseContext>().Database.Migrate();

            // services.AddDbContext<MyDatabaseContext>(options => {
            //     options.UseSqlServer(Configuration.GetConnectionString("MyDbConnection"));
            // });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Todos}/{action=Index}/{id?}");
            });
        }
    }
}
