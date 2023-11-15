using Dapper; 
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
            //C:\Users\30698\source\Advanced\DapperORMProject\DapperORMProject
            var configuration = new ConfigurationBuilder()
                                .AddJsonFile(@"C:\Users\30698\source\Advanced\DapperORMProject\DapperORMProject\appsettings.json") // Replace with the actual path to your appsettings.json file
                                .Build();

            IDbConnection db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            Wallet walletObj = new Wallet();


            Wallet wallet = new Wallet()
            {
              
                Holder = "Ibrahim",
                Balance = 30000,
            };

            Wallet walletUpdate = new Wallet()
            {
                Id = 10,
                Holder = "Ibrahim Update",
                Balance = 30000,
            };
            //walletObj.walletToInsert(db, wallet);

            walletObj.UpdateWallet(db,walletUpdate);

            //walletObj.DeleteWalletRecord(db, 30);


            //walletObj.TransferFunds(db,6,4,500);

            //walletObj.InsertRecordWithIdentity(db, wallet);

            //walletObj.GetMinMaxWalletBalance(db);

            

            walletObj.PrintData(db);






        }


    }
}
