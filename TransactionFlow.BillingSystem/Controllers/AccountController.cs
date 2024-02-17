using Microsoft.AspNetCore.Mvc;
using TransactionFlow.BillingSystem.Models.Dtos;
using TransactionFlow.BillingSystem.Services.Abstraction;
using TransactionFlow.Core.Constants;

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
    
    [Route(nameof(DeactivateAccountAsync))]
    [HttpPost]
    public async Task<IActionResult> DeactivateAccountAsync(int accountId)
    {
        var result = await _accountService.DeactivateAccountAsync(accountId);
        if (result.IsFailed)
        {
            return BadRequest(result.Reasons);
        }

        return Ok();
    }
    
    [Route(nameof(ActivateAccountAsync))]
    [HttpPost]
    public async Task<IActionResult> ActivateAccountAsync(int accountId)
    {
        var result = await _accountService.ActivateAccountAsync(accountId);
        if (result.IsFailed)
        {
            return BadRequest(result.Reasons);
        }

        return Ok();
    }
    
    //CtoC - From customer to customer. 
    //Deducts amount + fee from sender customer's main account and adds to receiver customer's main account.
    [Route(nameof(TransferMoneyCToCAsync))]
    [HttpPost]
    public async Task<IActionResult> TransferMoneyCToCAsync(TransferDto transferDto)
    {
        var result = await _transferService.TransferMoneyAsync(transferDto,TransferConditions.CToC);
        if (result.IsFailed)
        {
            return BadRequest(result.Reasons);
        }

        return Ok();
    }
    
    //AToA - From account to account.
    //Deducts amount + fee from sender customer's certain account and adds to
    //receiver customer's certain account.
    [Route(nameof(TransferMoneyAToAAsync))]
    [HttpPost]
    public async Task<IActionResult> TransferMoneyAToAAsync(TransferDto transferDto)
    {
        var result = await _transferService.TransferMoneyAsync(transferDto,TransferConditions.AToA);
        if (result.IsFailed)
        {
            return BadRequest(result.Reasons);
        }

        return Ok();
    }
    
    //CToA - From account to account.
    //Deducts amount + fee from sender customer's main account and adds to
    //receiver customer's certain account.
    [Route(nameof(TransferMoneyCToAsync))]
    [HttpPost]
    public async Task<IActionResult> TransferMoneyCToAsync(TransferDto transferDto)
    {
        var result = await _transferService.TransferMoneyAsync(transferDto,TransferConditions.CToA);
        if (result.IsFailed)
        {
            return BadRequest(result.Reasons);
        }

        return Ok();
    }
    
    //AToC - From account to account.
    //Deducts amount + fee from sender customer's certain account and adds to
    //receiver customer's main account.
    [Route(nameof(TransferMoneyAToCAsync))]
    [HttpPost]
    public async Task<IActionResult> TransferMoneyAToCAsync(TransferDto transferDto)
    {
        var result = await _transferService.TransferMoneyAsync(transferDto,TransferConditions.AToC);
        if (result.IsFailed)
        {
            return BadRequest(result.Reasons);
        }

        return Ok();
    }
}