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
    public IActionResult GetCustomerTransactions(int customerId,int count)
    {
        var result = _transactionService.GetCustomerTransactions(customerId, count);
        if (result.IsFailed)
        {
            return BadRequest(result.Reasons);
        }

        return Ok(result.Value);
    }
    
    [Route(nameof(GetSentCustomerTransactions))]
    [HttpGet]
    public IActionResult GetSentCustomerTransactions(int customerId,int count)
    {
        var result = _transactionService.GetSentTransactions(customerId, count);
        if (result.IsFailed)
        {
            return BadRequest(result.Reasons);
        }

        return Ok(result.Value);
    }
    
    [Route(nameof(GetReceivedCustomerTransactions))]
    [HttpGet]
    public IActionResult GetReceivedCustomerTransactions(int customerId,int count)
    {
        var result = _transactionService.GetReceivedTransactions(customerId, count);
        if (result.IsFailed)
        {
            return BadRequest(result.Reasons);
        }

        return Ok(result.Value);
    }
    
    [Route(nameof(GetAccountTransactions))]
    [HttpGet]
    public IActionResult GetAccountTransactions(int accountId,int count)
    {
        var result = _transactionService.GetAccountTransactions(accountId, count);
        if (result.IsFailed)
        {
            return BadRequest(result.Reasons);
        }

        return Ok(result.Value);
    }
    
    [Route(nameof(GetSentAccountTransactions))]
    [HttpGet]
    public IActionResult GetSentAccountTransactions(int accountId,int count)
    {
        var result = _transactionService.GetSentAccountTransactions(accountId, count);
        if (result.IsFailed)
        {
            return BadRequest(result.Reasons);
        }

        return Ok(result.Value);
    }
    
    [Route(nameof(GetReceivedAccountTransactions))]
    [HttpGet]
    public IActionResult GetReceivedAccountTransactions(int accountId,int count)
    {
        var result = _transactionService.GetReceivedAccountTransactions(accountId, count);
        if (result.IsFailed)
        {
            return BadRequest(result.Reasons);
        }

        return Ok(result.Value);
    }
}