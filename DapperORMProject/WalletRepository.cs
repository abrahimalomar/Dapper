using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;

namespace DapperORMProject
{
    public class WalletRepository : IWalletRepository
    {
        private readonly IDbConnection _db;

        public WalletRepository(IDbConnection db)
        {
            _db = db;
        }
    
        public int CheckIfHolderExists(string holder)
        {
            var checkIfExistsQuery = "SELECT COUNT(*) FROM Wallets WHERE Holder = @Holder";
            return _db.QuerySingleOrDefault<int>(checkIfExistsQuery, new { Holder = holder });
        }
        public void DeleteWallet(int id)
        {
            var sql = "DELETE FROM Wallets WHERE Id = @Id;";
            var parameters = new { Id = id };

            try
            {
                int rowsAffected = _db.Execute(sql, parameters);

                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Record with ID {id} deleted successfully.");
                }
                else
                {
                    Console.WriteLine($"Record with ID {id} not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while deleting the record: {ex.Message}");
            }
        }

        public IEnumerable<Wallet> GetAllWallets()
        {
            var sql = "SELECT * FROM Wallets";
            return _db.Query<Wallet>(sql);
        }

        public (decimal MinBalance, decimal MaxBalance) GetMinMaxWalletBalance()
        {
            var sql = "SELECT MIN(Balance) AS MinBalance, MAX(Balance) AS MaxBalance FROM Wallets";
            return _db.QuerySingle<(decimal, decimal)>(sql);
        }

        public Wallet GetWalletById(int id)
        {
            var sql = "SELECT * FROM Wallets WHERE Id = @Id";
            return _db.QuerySingleOrDefault<Wallet>(sql, new { Id = id });
        }
        public int InsertRecordWithIdentity(Wallet wallet)
        {
            var checkIfExistsQuery = "SELECT COUNT(*) FROM Wallets WHERE Holder = @Holder";
            int count = _db.QuerySingleOrDefault<int>(checkIfExistsQuery, new { Holder = wallet.Holder });

            if (count > 0)
            {
                Console.WriteLine("The Holder name already exists in the table");
                return -1; 
            }

            var sqlInsert = "INSERT INTO Wallets (Holder, Balance) VALUES (@Holder, @Balance); SELECT CAST(SCOPE_IDENTITY() AS INT)";
            return _db.Query<int>(sqlInsert, wallet).Single();
        }

        public void InsertWallet(Wallet wallet)
        {
            try
            {
                if (CheckIfHolderExists(wallet.Holder) > 0)
                {
                    Console.WriteLine("The Holder name already exists in the table");
                }
                else
                {
                    var sqlInsert = "INSERT INTO Wallets (Holder, Balance) VALUES (@Holder, @Balance)";
                    int rowsAffected = _db.Execute(sqlInsert, new
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
        public void TransferFunds(int fromId, int toId, decimal amountToTransfer)
        {
            try
            {
                using (var transaction = _db.BeginTransaction())
                {
                    var walletFrom = _db.QuerySingleOrDefault<Wallet>("SELECT Id, Balance FROM Wallets WHERE Id = @Id", new { Id = fromId });
                    var walletTo = _db.QuerySingleOrDefault<Wallet>("SELECT Id, Balance FROM Wallets WHERE Id = @Id", new { Id = toId });

                    if (walletFrom != null && walletTo != null && walletFrom.Balance >= amountToTransfer)
                    {
                        _db.Execute("UPDATE Wallets SET Balance = @Balance WHERE Id = @Id", new { Id = walletFrom.Id, Balance = walletFrom.Balance - amountToTransfer });
                        _db.Execute("UPDATE Wallets SET Balance = @Balance WHERE Id = @Id", new { Id = walletTo.Id, Balance = walletTo.Balance + amountToTransfer });

                        transaction.Commit();
                        Console.WriteLine("Funds transfer successful.");
                    }
                    else
                    {
                        Console.WriteLine("Unable to transfer funds. Please check wallet availability and balance.");
                        transaction.Rollback();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while transferring funds: {ex.Message}");
            }
        }

        public void UpdateWallet(Wallet wallet)
            {
                var sql = "UPDATE Wallets SET Holder = @Holder, Balance = @Balance WHERE Id = @Id;";
                var parameters = new { Id = wallet.Id, Holder = wallet.Holder, Balance = wallet.Balance };

                int affectedRows = _db.Execute(sql, parameters);

                if (affectedRows > 0)
                {
                    Console.WriteLine($"Wallet with ID {wallet.Id} updated successfully.");
                }
                else
                {
                    Console.WriteLine($"No wallet found with ID {wallet.Id} to update.");
                }
            

        }
       


    }

}
