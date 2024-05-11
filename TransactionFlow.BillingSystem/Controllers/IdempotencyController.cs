using Microsoft.AspNetCore.Mvc;
using TransactionFlow.BillingSystem.Services.Abstraction;

namespace TransactionFlow.BillingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdempotencyController : ControllerBase
    {
        private readonly IIdempotencyService _idempotencyService;

        public IdempotencyController(IIdempotencyService idempotencyService)
        {
            _idempotencyService = idempotencyService;
        }

        [Route(nameof(GenerateKey))]
        [HttpGet]
        public IActionResult GenerateKey()
        {
            var result = _idempotencyService.GenerateKey();
            if (result.IsFailed)
            {
                return BadRequest(result.Reasons);
            }

            return Ok(result.Value);
        }
    }
}
