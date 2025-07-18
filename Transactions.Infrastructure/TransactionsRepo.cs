using Microsoft.EntityFrameworkCore;
using Transactions.Application.DTOs;
using Transactions.Application.Repositories;
using Transactions.Infrastructure.Models;
using Transaction = Transactions.Domain.Models.Transaction;
using DomainTransactionType = Transactions.Domain.TransactionType;
using Transactions.Application.Extensions;

namespace Transactions.Infrastructure.Repositories
{
    public class TransactionsRepo : ITransactionsRepo
    {
        private readonly TransactionDbContext _context;

        public TransactionsRepo(TransactionDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CreateTransactionDto dto)
        {
            var entity = new Transaction
            {
                Id = Guid.NewGuid(),
                CardNumber = dto.CardNumber,
                Date = dto.Date,
                Amount = dto.Amount,
                Type = (int?)dto.Type
            };

            _context.Transactions.Add(entity);
            await _context.SaveChangesAsync();
        }


        public async Task<List<Transaction>> GetAllAsync()
        {
            return await _context.Transactions.ToListAsync();
        }

        public async Task<List<Transaction>> GetPaymentsAsync()
        {
            return await _context.Transactions
                .Where(t => t.Type == 0)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalAmountAsync()
        {
            return await _context.Transactions.SumAsync(t => t.Amount ?? 0);
        }
    }
}

