using Microsoft.AspNetCore.Mvc;
using TransactionFlow.BillingSystem.Services.Abstraction;

namespace TransactionFlow.BillingSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TransactionsController:ControllerBase
{
    private ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [Route(nameof(GetTransactions))]
    [HttpGet]
    public IActionResult GetTransactions(int count)
    {
        var result = _transactionService.GetTransactions(count);
        if (result.IsFailed)
        {
            return BadRequest(result.Reasons);
        }

        return Ok(result.Value);
    }
    
    [Route(nameof(GetCustomerTransactions))]
    [HttpGet]
    public IActionResult GetCustomerTransactions(int count)
    {
        var result = _transactionService.GetTransactions(count);
        if (result.IsFailed)
        {
            return BadRequest(result.Reasons);
        }

        return Ok(result.Value);
    }
}