using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transactions.Application.DTOs;

namespace Transactions.Application.Services.Users
{
    public interface IUsersService
    {
        Task<IReadOnlyList<UserListItemDto>> GetUsersAsync(string? q, string? role, CancellationToken ct = default);

        Task<bool> LockAsync(string userId, TimeSpan? duration = null, CancellationToken ct = default);
        Task<bool> UnlockAsync(string userId, CancellationToken ct = default);
        Task<bool> DeleteAsync(string userId, CancellationToken ct = default);
    }
}
