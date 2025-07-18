using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transactions.Domain
{
    public enum TransactionType
    {
        Paiement,
        Retrait,
        Virement
    }

    public class Transaction
    {
        public Guid Id { get; set; }
        public string CardNumber { get; set; } = "";
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }

        public string MaskedCardNumber => $"{CardNumber.Substring(0, 4)}******{CardNumber.Substring(10)}";
    }
}