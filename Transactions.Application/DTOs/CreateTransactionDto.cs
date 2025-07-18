using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transactions.Domain;

namespace Transactions.Application.DTOs
{
    public class CreateTransactionDto
    {
        public string CardNumber { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public decimal Amount { get; set; }

        public TransactionType Type { get; set; }
    }
}
