using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using transactions.Transactions.Domain.Entities;
using transactions.Transactions.Infrastucture;

namespace transactions.Transactions.Infrastucture
{
   
   

    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            await _context.SaveChangesAsync();
        }

        public async Task<List<Transaction>> GetAllAsync()
        {
            return await _context.Transactions.ToListAsync();
        }

        public async Task<List<Transaction>> GetByTypeAsync(TransactionType type)
        {
            return await _context.Transactions
                                 .Where(t => t.Type == type)
                                 .ToListAsync();
        }

        public async Task<decimal> GetTotalAmountAsync()
        {
            return await _context.Transactions.SumAsync(t => t.Amount);
        }
    }

  
}
