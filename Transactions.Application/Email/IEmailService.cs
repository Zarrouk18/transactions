using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transactions.Application.Email
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string username, string email, string password);

    }
}
