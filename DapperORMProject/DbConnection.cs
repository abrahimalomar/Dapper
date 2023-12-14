using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;


namespace DapperORMProject
{
    public class DbConnection
    {

        public static IDbConnection GetDbConnection()
        {

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            return new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }



    }
}




