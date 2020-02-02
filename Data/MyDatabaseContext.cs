using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreSqlDb.Models
{
    public class MyDatabaseContext : DbContext
    {
        public MyDatabaseContext (DbContextOptions<MyDatabaseContext> options)
            : base(options)
        {
            var conn = Database.GetDbConnection() as System.Data.SqlClient.SqlConnection;
            if (conn != null) {
                string clientId = Environment.GetEnvironmentVariable("APP_CLIENT_ID");
                Microsoft.Azure.Services.AppAuthentication.AzureServiceTokenProvider tokenProvider;
                if (!String.IsNullOrEmpty(clientId)) {
                    // User assigned identity requires the Client ID to be specified, see:
                    // https://docs.microsoft.com/en-us/azure/key-vault/service-to-service-authentication#connection-string-support
                    tokenProvider = new Microsoft.Azure.Services.AppAuthentication.AzureServiceTokenProvider("RunAs=App;AppId="+clientId);
                } else {
                    tokenProvider = new Microsoft.Azure.Services.AppAuthentication.AzureServiceTokenProvider();
                }

                // Get AAD token when using SQL Database
                conn.AccessToken = tokenProvider.GetAccessTokenAsync("https://database.windows.net/").Result;
            }
        }

        public DbSet<DotNetCoreSqlDb.Models.Todo> Todo { get; set; }
    }
}
