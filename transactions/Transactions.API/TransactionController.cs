using transactions.Transactions.App;
using transactions.Transactions.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;   
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using transactions.Transactions.Domain.Interfaces;

namespace transactions.Transactions.API.Controllers
{

    [ApiController]
    [Route("transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _service;

        public TransactionsController(ITransactionService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Transaction transaction)
        {
            await _service.AddTransaction(transaction);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list.Select(t => new {
                t.Id,
                CardNumber = t.CardNumber.Substring(0, 4) + "******" + t.CardNumber.Substring(10),
                t.Date,
                t.Amount,
                t.Type
            }));
        }

        [HttpGet("payments")]
        public async Task<IActionResult> GetPayments()
        {
            var payments = await _service.GetPaymentsAsync();
            return Ok(payments.Select(t => new {
                t.Id,
                t.CardNumber ,
                t.Date,
                t.Amount,
                t.Type
            }));
        }

        [HttpGet("total")]
        public async Task<IActionResult> GetTotal()
        {
            var total = await _service.GetTotalAmount();
            return Ok(new { Total = total });
        }
    }
}
