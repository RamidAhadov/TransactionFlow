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
    private IIdempotencyService _idempotencyService;

    public AccountController(ITransferService transferService, IAccountService accountService, IIdempotencyService idempotencyService)
    {
        _transferService = transferService;
        _accountService = accountService;
        _idempotencyService = idempotencyService;
    }
    
    [Route(nameof(CreateAccount))]
    [HttpPost]
    public IActionResult CreateAccount([FromBody] int customerId)
    {
        var key = Request.Headers["Idempotency-key"].ToString();

        var idempotencyResult = _idempotencyService.Get(key);
        if (idempotencyResult == null)
        {
            var result = _accountService.CreateAccount(customerId);
            if (result.IsFailed)
            {
                return BadRequest(result.Reasons);
            }

            _idempotencyService.Set(key);
            return Ok();
        }

        return Ok();
    }

    [Route(nameof(DeleteAccountAsync))]
    [HttpPost]
    public async Task<IActionResult> DeleteAccountAsync([FromBody] int accountId)
    {
        var key = Request.Headers["Idempotency-key"].ToString();

        var idempotencyResult = _idempotencyService.Get(key);
        if (idempotencyResult == null)
        {
            var result = await _accountService.DeleteAccountAsync(accountId);
            if (result.IsFailed)
            {
                return BadRequest(result.Reasons);
            }

            _idempotencyService.Set(key);
            return Ok();
        }

        return Ok();
    }
    
    [Route(nameof(DeactivateAccountAsync))]
    [HttpPost]
    public async Task<IActionResult> DeactivateAccountAsync([FromBody] int accountId)
    {
        var key = Request.Headers["Idempotency-key"].ToString();

        var idempotencyResult = _idempotencyService.Get(key);
        if (idempotencyResult == null)
        {
            var result = await _accountService.DeactivateAccountAsync(accountId);
            if (result.IsFailed)
            {
                return BadRequest(result.Reasons);
            }

            _idempotencyService.Set(key);
            return Ok();
        }

        return Ok();
    }
    
    [Route(nameof(ActivateAccountAsync))]
    [HttpPost]
    public async Task<IActionResult> ActivateAccountAsync([FromBody] int accountId)
    {
        var key = Request.Headers["Idempotency-key"].ToString();

        var idempotencyResult = _idempotencyService.Get(key);
        if (idempotencyResult == null)
        {
            var result = await _accountService.ActivateAccountAsync(accountId);
            if (result.IsFailed)
            {
                return BadRequest(result.Reasons);
            }

            _idempotencyService.Set(key);
            return Ok();
        }

        return Ok();
    }
    
    //CtoC - From customer to customer. 
    //Deducts amount + fee from sender customer's main account and adds to receiver customer's main account.
    [Route(nameof(TransferMoneyCToCAsync))]
    [HttpPost]
    public async Task<IActionResult> TransferMoneyCToCAsync([FromBody] TransferDto transferDto)
    {
        var key = Request.Headers["Idempotency-key"].ToString();

        var idempotencyResult = _idempotencyService.Get(key);
        if (idempotencyResult == null)
        {
            var result = await _transferService.TransferMoneyAsync(transferDto,TransferConditions.CToC);
            if (result.IsFailed)
            {
                return BadRequest(result.Reasons);
            }

            _idempotencyService.Set(key);
            return Ok();
        }

        return Ok();
    }
    
    //AToA - From account to account.
    //Deducts amount + fee from sender customer's certain account and adds to
    //receiver customer's certain account.
    [Route(nameof(TransferMoneyAToAAsync))]
    [HttpPost]
    public async Task<IActionResult> TransferMoneyAToAAsync([FromBody] TransferDto transferDto)
    {
        var key = Request.Headers["Idempotency-key"].ToString();

        var idempotencyResult = _idempotencyService.Get(key);
        if (idempotencyResult == null)
        {
            var result = await _transferService.TransferMoneyAsync(transferDto,TransferConditions.AToA);
            if (result.IsFailed)
            {
                return BadRequest(result.Reasons);
            }

            _idempotencyService.Set(key);
            return Ok();
        }

        return Ok();
    }
    
    //CToA - From account to account.
    //Deducts amount + fee from sender customer's main account and adds to
    //receiver customer's certain account.
    [Route(nameof(TransferMoneyCToAsync))]
    [HttpPost]
    public async Task<IActionResult> TransferMoneyCToAsync([FromBody] TransferDto transferDto)
    {
        var key = Request.Headers["Idempotency-key"].ToString();

        var idempotencyResult = _idempotencyService.Get(key);
        if (idempotencyResult == null)
        {
            var result = await _transferService.TransferMoneyAsync(transferDto,TransferConditions.CToA);
            if (result.IsFailed)
            {
                return BadRequest(result.Reasons);
            }

            _idempotencyService.Set(key);
            return Ok();
        }

        return Ok();
    }
    
    //AToC - From account to account.
    //Deducts amount + fee from sender customer's certain account and adds to
    //receiver customer's main account.
    [Route(nameof(TransferMoneyAToCAsync))]
    [HttpPost]
    public async Task<IActionResult> TransferMoneyAToCAsync([FromBody] TransferDto transferDto)
    {
        var key = Request.Headers["Idempotency-key"].ToString();

        var idempotencyResult = _idempotencyService.Get(key);
        if (idempotencyResult == null)
        {
            var result = await _transferService.TransferMoneyAsync(transferDto,TransferConditions.AToC);
            if (result.IsFailed)
            {
                return BadRequest(result.Reasons);
            }

            _idempotencyService.Set(key);
            return Ok();
        }

        return Ok();
    }
}