using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using transactions.Transactions.Domain.Entities;

namespace transactions.Transactions.Infrastucture
{
    public class AppDbContext : DbContext 

    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Transaction> Transactions => Set<Transaction>();
    }
}
