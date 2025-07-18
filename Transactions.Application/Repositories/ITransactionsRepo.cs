using Transactions.Application.DTOs;
using Transactions.Application.Extensions;
using Transactions.Domain.Models;   

namespace Transactions.Application.Repositories;

public interface ITransactionsRepo
{
    Task AddAsync(CreateTransactionDto dto);
    Task<List<Transaction>> GetAllAsync();
    Task<List<Transaction>> GetPaymentsAsync();
    Task<decimal> GetTotalAmountAsync();
}
