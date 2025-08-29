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
                Date = DateTime.Now,
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
        public async Task<bool> UpdateAsync(Guid id, TransactionDto dto)
        {
            var entity = await _context.Transactions.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return false;

            
            entity.CardNumber = dto.CardNumber!;
            entity.Amount = dto.Amount!.Value;
            entity.Type = (int?)dto.Type!.Value;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _context.Transactions.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return false;

            _context.Transactions.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(List<Transaction> Items, int TotalCount)> GetPagedAsync(
           int page, int pageSize, string? q, int? type, string? sort)
        {
            var qry = _context.Transactions.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var qq = q.ToLower();
                qry = qry.Where(t =>
                    (t.CardNumber ?? "").ToLower().Contains(qq) ||
                    (t.Amount ?? 0).ToString().Contains(qq) ||
                    (t.Date != null && t.Date.Value.ToString()!.ToLower().Contains(qq))
                );
            }

            if (type.HasValue)
                qry = qry.Where(t => t.Type == type.Value);

            qry = (sort?.ToLowerInvariant()) switch
            {
                "amount_asc" => qry.OrderBy(t => t.Amount),
                "amount_desc" => qry.OrderByDescending(t => t.Amount),
                "date_asc" => qry.OrderBy(t => t.Date),
                _ => qry.OrderByDescending(t => t.Date) // default
            };

            var total = await qry.CountAsync();
            var items = await qry.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (items, total);
        }
    }
}

