using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transactions.Application.Extensions
{
    public static class StringExtensions
    {
        public static string MaskCardNumber(this string? cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length < 16)
                return "************";

            return $"{cardNumber.Substring(0, 4)}******{cardNumber.Substring(10)}";
        }
    }
}
