using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transactions.Application.DTOs;

namespace Transactions.Application.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RefreshAsync(RefreshRequest request);
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
    }
}
