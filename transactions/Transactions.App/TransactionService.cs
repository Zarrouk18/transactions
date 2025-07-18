using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using transactions.Transactions.Domain.Entities;
using transactions.Transactions.Domain.Interfaces;


namespace transactions.Transactions.App
{
    public class TransactionService : ITransactionService
    {
        
        private readonly ITransactionRepository _repository;

        public TransactionService(ITransactionRepository repository)
        {
            _repository = repository;
        }

        public async Task  AddTransaction(Transaction transaction)
        {
            await _repository.AddAsync(transaction);
        }

        public async Task<List<Transaction>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<List<Transaction>> GetPaymentsAsync()
        {
            return await _repository.GetByTypeAsync(TransactionType.Paiement);
        }

        public async Task<decimal> GetTotalAmount()
        {
            return await _repository.GetTotalAmountAsync();
        }
  

      
    }
}
