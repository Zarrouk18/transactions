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
            return Ok(new { message = "Transaction enregistrée avec succès." });
        }

        [Authorize]
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll([FromQuery] TransactionQuery query)
        {
            var result = await _service.GetPagedAsync(query);
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

        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] TransactionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var ok = await _service.UpdateAsync(id, dto);
                if (!ok) return NotFound(new { message = "Transaction not found." });
                return Ok(new { message = "Transaction updated." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound(new { message = "Transaction not found." });
            return Ok(new { message = "Transaction deleted." });
        }
        [Authorize] // or [Authorize(Roles="ADMIN")] if you prefer
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var all = await _service.GetAllAsync();
            var item = all.FirstOrDefault(t => t.Id == id);
            return item is null ? NotFound(new { message = "Transaction not found." }) : Ok(item);
        }
    }
}
