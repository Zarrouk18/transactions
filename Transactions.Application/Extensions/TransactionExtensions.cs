using Transactions.Application.DTOs;
using Transactions.Domain;
using Transactions.Domain.Models;
using Transaction= Transactions.Domain.Models.Transaction;


namespace Transactions.Application.Extensions
{
    public static class TransactionExtensions
    {
        public static TransactionDto ToTransactionDto(this Transaction t)
        {
            return new TransactionDto
            {
                Id = t.Id,
                CardNumber = t.CardNumber.MaskCardNumber(),
                Date = t.Date,
                Amount = t.Amount,
                Type = (TransactionType?)t.Type
            };
        }

        public static List<TransactionDto> ToTransactionDtoList(this List<Transaction> transactions)
        {
            return transactions.Select(t => t.ToTransactionDto()).ToList();
        }
    }
}
