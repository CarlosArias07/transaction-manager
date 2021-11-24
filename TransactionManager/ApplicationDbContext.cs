using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransactionManager.Models;

namespace TransactionManager
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base (options)
        {

        }

        public DbSet<Transaction> Transactions { get; set; }
    }
}
