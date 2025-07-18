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
    }
}
