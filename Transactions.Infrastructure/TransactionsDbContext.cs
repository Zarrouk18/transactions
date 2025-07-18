using Microsoft.EntityFrameworkCore;
using Transactions.Domain;

namespace Transactions.Infrastructure
{
    public class TransactionsDbContext : DbContext
    {
        public TransactionsDbContext(DbContextOptions<TransactionsDbContext> options) : base(options) { }

        public DbSet<Transaction> Transactions { get; set; }
    }
}
