using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Linq;

namespace DapperORMProject
{
    class Program
    {
        static void Main(string[] args)
        {

           
            using (IDbConnection dbConnection = DbConnection.GetDbConnection())
            {
         

                try
                {

                    IWalletRepository walletRepository = new WalletRepository(dbConnection);


                    walletRepository.GetAllWallets();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
    
        }


    }

  
}
