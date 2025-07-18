using System;
using System.Collections.Generic;

namespace Transactions.Domain.Models;

public partial class Transaction
{
    public Guid Id { get; set; }

    public string? CardNumber { get; set; }

    public DateTime? Date { get; set; }

    public decimal? Amount { get; set; }

    public int? Type { get; set; }
}
