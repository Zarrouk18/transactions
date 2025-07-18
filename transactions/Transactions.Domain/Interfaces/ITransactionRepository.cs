using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using transactions.Transactions.Domain.Entities;



public interface ITransactionRepository
    {
        Task AddAsync(Transaction transaction);
        Task<List<Transaction>> GetAllAsync();
        Task<List<Transaction>> GetByTypeAsync(TransactionType type);
        Task<decimal> GetTotalAmountAsync();
    }



