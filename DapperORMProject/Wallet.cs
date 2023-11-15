using Dapper;
using System;
using System.Data;
using System.Linq;
using System.Transactions;

namespace DapperORMProject
{
    public class Wallet
    {
        public int Id { get; set; }
        public string Holder { get; set; }
        public decimal? Balance { get; set; }

        public override string ToString()
        {
            return $"[{Id}] {Holder} ({Balance:C})";
        }
        public void walletToInsert(IDbConnection db, Wallet wallet)
        {
            try
            {
              
                var checkIfExistsQuery = "SELECT COUNT(*) FROM Wallets WHERE Holder = @Holder";
                int count = db.QuerySingleOrDefault<int>(checkIfExistsQuery, new { Holder = wallet.Holder });

                if (count > 0)
                {
                    Console.WriteLine("The name Holder already exists in the table");
                }
                else
                {
                    
                    var sqlInsert = "INSERT INTO Wallets (Holder, Balance) " +
                                    "VALUES (@Holder, @Balance)";

                    int rowsAffected = db.Execute(sqlInsert, new
                    {
                        Holder = wallet.Holder,
                        Balance = wallet.Balance
                    });

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Wallet record inserted successfully");
                    }
                    else
                    {
                        Console.WriteLine("No records were inserted");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while inserting the wallet record: {ex.Message}");
            }
        }
        public void UpdateWallet(IDbConnection db,Wallet walletToUpdate)
        {
            try
            {
                var sql = "UPDATE Wallets SET Holder = @Holder, Balance = @Balance " +
                          "WHERE Id = @Id;";

                var parameters = new
                {
                    Id = walletToUpdate.Id,
                    Holder = walletToUpdate.Holder,
                    Balance = walletToUpdate.Balance
                };

                int affectedRows = db.Execute(sql, parameters);

                if (affectedRows > 0)
                {
                    Console.WriteLine($"Wallet with ID {walletToUpdate.Id} updated successfully.");
                }
                else
                {
                    Console.WriteLine($"No wallet found with ID {walletToUpdate.Id} to update.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while updating the wallet: {ex.Message}");
            }

        }
        public void DeleteWalletRecord(IDbConnection db, int Id)
        {

            var sql = "DELETE FROM Wallets WHERE Id = @Id;";

            var parameters = new
            {
                Id = Id
            };
            //Console.WriteLine("parameters : " + parameters);
            //Console.WriteLine("Id : " + Id);
            try
            {
                int rowsAffected = db.Execute(sql, parameters);

                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Record with ID {Id} deleted successfully.");
                }
                else
                {
                    Console.WriteLine($"Record with ID {Id} not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while deleting the record: {ex.Message}");
            }

        }
        public void TransferFunds(IDbConnection db, int FromId,int ToId, decimal amountToTranfer)
        {
            try
            {
                using (var transactionScope = new TransactionScope())
                {
                    // Transfer 1000
                    // From: 8   Ibrahim   20000
                    // To:  4   Ali   5400

                    // = 1000m;
                    // Ibrahim
                    var walletFrom = db.QuerySingle<Wallet>("SELECT * FROM Wallets WHERE Id = @Id", new { Id =FromId  });
                    
                    //Ali
                    var walletTo = db.QuerySingle<Wallet>("SELECT * FROM Wallets WHERE Id = @Id", new { Id = ToId });

                    if (walletFrom != null && walletTo != null)
                    {
                        db.Execute("UPDATE Wallets SET Balance = @Balance WHERE Id = @Id",
                            new
                            {
                                Id = walletFrom.Id,
                                Balance = walletFrom.Balance - amountToTranfer
                            });

                        db.Execute("UPDATE Wallets SET Balance = @Balance WHERE Id = @Id",
                            new
                            {
                                Id = walletTo.Id,
                                Balance = walletTo.Balance + amountToTranfer
                            });

                        transactionScope.Complete();
                        Console.WriteLine("Funds transfer successful.");
                    }
                    else
                    {
                        Console.WriteLine("One or both of the wallets do not exist.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while transferring funds: {ex.Message}");
            }
        }
        public void InsertRecordWithIdentity(IDbConnection db, Wallet walletToInsert)
        {
            try
            {
           
                var checkIfExistsQuery = "SELECT COUNT(*) FROM Wallets WHERE Holder = @Holder";
                int count = db.QuerySingleOrDefault<int>(checkIfExistsQuery,
                    new Wallet
                    {
                        Holder = walletToInsert.Holder,
                    });
                if (count>0)
                {
                    Console.WriteLine("The name Holder already exists in the table");
                }
                else
                {
                    var sqlInsertRid = "INSERT INTO Wallets (Holder, Balance) " +
                        "VALUES (@Holder, @Balance)" +
                        "SELECT CAST(SCOPE_IDENTITY() AS INT)";

                    var parameters = new Wallet
                    {
                        Holder = walletToInsert.Holder,
                        Balance = walletToInsert.Balance
                    };

                    walletToInsert.Id = db.Query<int>(sqlInsertRid, parameters).Single();
                    Console.WriteLine("ID " + walletToInsert.Id + " " + walletToInsert.Holder);

                    Console.WriteLine(walletToInsert);

                    Console.WriteLine("Record inserted successfully");
                }
              
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while inserting the record: {ex.Message}");
            }

        }
        public void GetMinMaxWalletBalance(IDbConnection db)
        {

            //Execute mulit Query
            var sql = "SELECT MIN(Balance) AS MinBalance, MAX(Balance) AS MaxBalance FROM Wallets";

            var result = db.QuerySingle<(decimal MinBalance, decimal MaxBalance)>(sql);

            Console.WriteLine($"Minimum Balance: {result.MinBalance}");
            Console.WriteLine($"Maximum Balance: {result.MaxBalance}");

           
        }
        public void PrintData(IDbConnection db)
        {
            var sql = "SELECT * FROM WALLETS";

            //Console.WriteLine("---------------- using Dynamic Query -------------");
            //var resultAsDynamic = db.Query(sql);

            //foreach (var item in resultAsDynamic)
            //    Console.WriteLine(item);

            //Console.WriteLine("---------------- using Typed Query -------------");
            var wallets = db.Query<Wallet>(sql);

            foreach (var wallet in wallets)
                Console.WriteLine(wallet);
        }


    }
}