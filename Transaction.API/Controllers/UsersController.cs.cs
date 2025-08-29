using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Transactions.Application.DTOs.Users;
using Transactions.Application.Services.Users;

namespace Transaction.API.Controllers
{
    [ApiController]
    [Route("users")]
    [Authorize(Roles = "ADMIN")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _users;

        public UsersController(IUsersService users)
        {
            _users = users;
        }

        // GET /users?q=&role=
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? q, [FromQuery] string? role, CancellationToken ct)
        {
            var list = await _users.GetUsersAsync(q, role, ct);
            return Ok(list);
        }

        // POST /users/{id}/lock
        [HttpPost("{id}/lock")]
        public async Task<IActionResult> Lock([FromRoute] string id, [FromBody] LockUserRequest? body, CancellationToken ct)
        {
            var minutes = body?.Minutes;
            var ok = await _users.LockAsync(id, minutes.HasValue ? TimeSpan.FromMinutes(minutes.Value) : null, ct);
            return ok ? Ok(new { message = "User locked." }) : NotFound(new { message = "User not found." });
        }

        // POST /users/{id}/unlock
        [HttpPost("{id}/unlock")]
        public async Task<IActionResult> Unlock([FromRoute] string id, CancellationToken ct)
        {
            var ok = await _users.UnlockAsync(id, ct);
            return ok ? Ok(new { message = "User unlocked." }) : NotFound(new { message = "User not found." });
        }

        // DELETE /users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id, CancellationToken ct)
        {
            // Prevent self-delete (optional but recommended)
            var me = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(me) && string.Equals(me, id, StringComparison.Ordinal))
                return BadRequest(new { message = "You cannot delete your own account." });

            var ok = await _users.DeleteAsync(id, ct);
            return ok ? Ok(new { message = "User deleted." }) : NotFound(new { message = "User not found." });
        }
    }
}
