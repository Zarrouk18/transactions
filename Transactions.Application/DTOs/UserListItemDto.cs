using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transactions.Application.DTOs
{
    public class UserListItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string? Username { get; set; }
        public string? Email { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
    }
}
