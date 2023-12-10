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


    }
}