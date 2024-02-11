using Microsoft.AspNetCore.Mvc;
using TransactionFlow.BillingSystem.Services.Abstraction;

namespace TransactionFlow.BillingSystem.Controllers;


[Route("api/[controller]")]
[ApiController]
public class AccountController:ControllerBase
{
    private ITransferService _transferService;
    private IAccountService _accountService;

    public AccountController(ITransferService transferService, IAccountService accountService)
    {
        _transferService = transferService;
        _accountService = accountService;
    }

    [Route(nameof(CreateAccountAsync))]
    [HttpPost]
    public async Task<IActionResult> CreateAccountAsync(int customerId)
    {
        var result = await _accountService.CreateAccountAsync(customerId);
        if (result.IsFailed)
        {
            return BadRequest(result.Reasons);
        }

        return Ok();
    }

    [Route(nameof(DeleteAccountAsync))]
    [HttpPost]
    public async Task<IActionResult> DeleteAccountAsync(int accountId)
    {
        var result = await _accountService.DeleteAccountAsync(accountId);
        if (result.IsFailed)
        {
            return BadRequest(result.Reasons);
        }

        return Ok();
    }
    
    
    [Route(nameof(TransferMoneyAsync))]
    [HttpPost]
    public async Task<IActionResult> TransferMoneyAsync(int sender, int receiver, decimal amount, decimal fee)
    {
        var result = await _transferService.TransferMoneyAsync(sender, receiver, amount, fee);
        if (result.IsFailed)
        {
            return BadRequest(result.Reasons);
        }

        return Ok();
    }
}