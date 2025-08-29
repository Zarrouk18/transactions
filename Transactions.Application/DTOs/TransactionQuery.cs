using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transactions.Application.DTOs
{
    public class TransactionQuery
    {
        public int Page { get; set; } = 1;         // 1-based
        public int PageSize { get; set; } = 10;    // default 10, we’ll clamp
        public string? Q { get; set; }             // search (card/amount/date)
        public int? Type { get; set; }             // 0/1/2
        public string? Sort { get; set; } = "date_desc"; // date_asc|date_desc|amount_asc|amount_desc
    }
}
