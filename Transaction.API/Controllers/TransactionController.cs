using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transactions.Application.DTOs;
using Transactions.Application.Services.TransactionsServices;

namespace Transaction.API.Controllers
{
    [ApiController]
    [Route("transactions")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionsService _service;

        public TransactionController(ITransactionsService service)
        {
            _service = service;
        }

        [Authorize(Roles ="ADMIN")]
        [HttpPost("addTransaction")]
        public async Task<IActionResult> Post([FromBody] CreateTransactionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.AddTransaction(dto);
            return Ok("Transaction enregistrée avec succès.");
        }

        //[Authorize]
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }
        
        [Authorize(Roles ="USER")]
        [HttpGet("payments")]
        public async Task<IActionResult> GetPayments()
        {
            var result = await _service.GetPaymentsAsync();
            return Ok(result);
        }

        [HttpGet("total")]
        public async Task<IActionResult> GetTotalAmount()
        {
            var total = await _service.GetTotalAmount();
            return Ok(new { TotalAmount = total });
        }

    }
}
