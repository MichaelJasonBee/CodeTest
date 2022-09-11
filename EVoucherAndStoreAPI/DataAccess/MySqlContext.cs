using EVoucherAndStoreAPI.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVoucherAndStoreAPI.DataAccess
{
    public class MySqlContext : DbContext
    {
        public MySqlContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<EVouchers> EVouchers { get; set; }
        public DbSet<PaymenMethods> PaymentMethods { get; set; }
        public DbSet<TransactionModel> Transactions { get; set; }
    }
}
