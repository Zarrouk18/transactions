using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Transactions.Application.DTOs;
using Transactions.Application.Services.Users;
using Transactions.Domain;

namespace Transactions.Infrastructure
{
    public class UsersService : IUsersService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IReadOnlyList<UserListItemDto>> GetUsersAsync(string? q, string? role, CancellationToken ct = default)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var ql = q.ToLower();
                query = query.Where(u =>
                    (u.UserName != null && u.UserName.ToLower().Contains(ql)) ||
                    (u.Email != null && u.Email.ToLower().Contains(ql)));
            }

            var users = await query
                .OrderBy(u => u.UserName)
                .ToListAsync(ct);

            var list = new List<UserListItemDto>(users.Count);

            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);

                // optional server-side role filter
                if (!string.IsNullOrWhiteSpace(role) &&
                    !roles.Any(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                list.Add(new UserListItemDto
                {
                    Id = u.Id,
                    Username = u.UserName,
                    Email = u.Email,
                    Roles = roles,
                    LockoutEnabled = u.LockoutEnabled,
                    LockoutEnd = u.LockoutEnd
                });
            }

            return list;
        }
        public async Task<bool> LockAsync(string userId, TimeSpan? duration = null, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return false;

            // ensure lockout enabled on the user
            await _userManager.SetLockoutEnabledAsync(user, true);

            // default: lock for 1 day if no duration provided
            var until = DateTimeOffset.UtcNow.Add(duration ?? TimeSpan.FromDays(1));
            var res = await _userManager.SetLockoutEndDateAsync(user, until);
            return res.Succeeded;
        }

        public async Task<bool> UnlockAsync(string userId, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return false;

            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
            await _userManager.ResetAccessFailedCountAsync(user);
            return true;
        }

        public async Task<bool> DeleteAsync(string userId, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return false;

            var res = await _userManager.DeleteAsync(user);
            return res.Succeeded;
        }
    }
}
