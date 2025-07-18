using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Transactions.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }  
    }
}
