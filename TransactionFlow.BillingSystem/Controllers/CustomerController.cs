using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using NLog;
using NuGet.Protocol;
using TransactionFlow.BillingSystem.Models.Dtos;
using TransactionFlow.BillingSystem.Services.Abstraction;

namespace TransactionFlow.BillingSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController:ControllerBase
{
    private IAccountService _accountService;
    private IIdempotencyService _idempotencyService;

    public CustomerController(IAccountService accountService, IIdempotencyService idempotencyService)
    {
        _accountService = accountService;
        _idempotencyService = idempotencyService;
    }

    [Route(nameof(GetCustomers))]
    [HttpGet]
    public IActionResult GetCustomers()
    {
        var result = _accountService.GetAllCustomers();
        if (result.IsFailed)
        {
            return BadRequest(result.Reasons);
        }

        return Ok(result.Value);
    }

    [Route(nameof(UpdateCustomer))]
    [HttpPost]
    public IActionResult UpdateCustomer(int customerId, CustomerDto customer)
    {
        var key = Request.Headers["Idempotency-key"].ToString();

        var idempotencyResult = _idempotencyService.Get(key);
        if (idempotencyResult.IsFailed)
        {
            return BadRequest(idempotencyResult.Reasons);
        }
        
        if (idempotencyResult.Value == null)
        {
            var result = _accountService.UpdateCustomer(customerId, customer);
            if (result.IsFailed)
            {
                return BadRequest(result.Reasons);
            }

            _idempotencyService.Set(Request, HttpStatusCode.OK, customer);

            return Ok();
        }

        return Ok();
    }
    
    [Route(nameof(CreateCustomerAsync))]
    [HttpPost]
    public async Task<IActionResult> CreateCustomerAsync([FromBody] CustomerDto customer)
    {
        var key = Request.Headers["Idempotency-key"].ToString();

        var idempotencyResult = _idempotencyService.Get(key);
        if (idempotencyResult.IsFailed)
        {
            return BadRequest(idempotencyResult.Reasons);
        }
        
        if (idempotencyResult.Value == null)
        {
            var result = await _accountService.CreateCustomerAsync(customer);
            if (result.IsFailed)
            {
                return BadRequest(result.Reasons);
            }

            _idempotencyService.Set(Request, HttpStatusCode.OK, customer);

            return Ok();
        }

        return Ok();
    }
    
    [Route(nameof(DeleteCustomerAsync))]
    [HttpPost]
    public async Task<IActionResult> DeleteCustomerAsync([FromBody] int customerId)
    {
        var key = Request.Headers["Idempotency-key"].ToString();

        var idempotencyResult = _idempotencyService.Get(key);
        if (idempotencyResult.IsFailed)
        {
            return BadRequest(idempotencyResult.Reasons);
        }
        
        if (idempotencyResult.Value == null)
        {
            var result = await _accountService.DeleteCustomerAsync(customerId);
            if (result.IsFailed)
            {
                return BadRequest(result.Reasons);
            }

            _idempotencyService.Set(Request, HttpStatusCode.OK, customerId);

            return Ok();
        }

        return Ok();
    }

    [HttpGet]
    [Route(nameof(GetCustomerById))]
    public IActionResult GetCustomerById(int id)
    {
        var result = _accountService.GetCustomer(id);
        if (result.IsFailed)
        {
            return BadRequest(result.Reasons);
        }
        
        return Ok();
    }
}