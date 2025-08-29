using Transactions.Application.Services.TransactionsServices;
using Transactions.Application.DTOs;
using Transactions.Application.Repositories;
using Transactions.Application.Extensions;
using System.ComponentModel;

namespace Transactions.Application
{
    public class TransactionsService : ITransactionsService
    {
        private readonly ITransactionsRepo _repo;

        public TransactionsService(ITransactionsRepo repo)
        {
            _repo = repo;
        }

        public async Task AddTransaction(CreateTransactionDto dto)
        {
            if (dto.Amount <= 0)
                throw new ArgumentException("Amount cannot be negative.");
            await _repo.AddAsync(dto);


        }

        public async Task<List<TransactionDto>> GetAllAsync()
        {
            var transactions = await _repo.GetAllAsync();
            return  transactions.ToTransactionDtoList();
        }

        public async Task<List<TransactionDto>> GetPaymentsAsync()
        {
           var payments= await _repo.GetPaymentsAsync();
            return payments.ToTransactionDtoList();
        }

        public async Task<decimal> GetTotalAmount()
        {
            return await _repo.GetTotalAmountAsync();
        }
        public async Task<bool> UpdateAsync(Guid id, TransactionDto dto)
        {
            // We expect CardNumber, Amount, Type for an update.
            if (dto.Amount.HasValue && dto.Amount.Value < 0)
                throw new ArgumentException("Amount cannot be negative.");

            // Date is NOT changed here (kept as originally set on create)
            return await _repo.UpdateAsync(id, dto);
        }

        public async Task<bool> DeleteAsync(Guid id)
        { 
            return await _repo.DeleteAsync(id);
        }

        public async Task<PaginatedResult<TransactionDto>> GetPagedAsync(TransactionQuery query)
        {
            var page = query.Page < 1 ? 1 : query.Page;
            var size = query.PageSize < 5 ? 5 : (query.PageSize > 100 ? 100 : query.PageSize);

            var (items, total) = await _repo.GetPagedAsync(page, size, query.Q, query.Type, query.Sort);

            return new PaginatedResult<TransactionDto>
            {
                Items = items.ToTransactionDtoList(),
                TotalCount = total,
                Page = page,
                PageSize = size
            };
        }

    }
}
