using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace transactions.Transactions.Domain.Entities

{
    public class Transaction
    {
        public Guid Id { get; set; }
        public string CardNumber { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
    }
}
