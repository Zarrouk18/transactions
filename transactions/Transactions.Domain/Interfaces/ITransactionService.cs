using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using transactions.Transactions.Domain.Entities;

namespace transactions.Transactions.Domain.Interfaces;
   
    public interface ITransactionService
    {
        Task AddTransaction(Transaction transaction);
    Task<List<Transaction>> GetAllAsync();
    Task<List<Transaction>> GetPaymentsAsync();
    Task<decimal> GetTotalAmount();
    }

