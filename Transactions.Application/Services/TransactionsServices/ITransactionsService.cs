using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transactions.Application.DTOs;

namespace Transactions.Application.Services.TransactionsServices
{
    public interface ITransactionsService
    {
        Task AddTransaction(CreateTransactionDto transaction);
        Task<List<TransactionDto>> GetAllAsync();
        Task<List<TransactionDto>> GetPaymentsAsync();
        Task<decimal> GetTotalAmount();
        Task<bool> UpdateAsync(Guid id, TransactionDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<PaginatedResult<TransactionDto>> GetPagedAsync(TransactionQuery query);
    }
}
