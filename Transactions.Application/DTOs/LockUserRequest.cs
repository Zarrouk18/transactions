using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transactions.Application.DTOs.Users
{
    public sealed class LockUserRequest
    {
        public int? Minutes { get; set; }
    }
}
