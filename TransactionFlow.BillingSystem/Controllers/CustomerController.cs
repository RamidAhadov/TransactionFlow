using Microsoft.AspNetCore.Mvc;
using TransactionFlow.BillingSystem.Models.Dtos;
using TransactionFlow.BillingSystem.Services.Abstraction;

namespace TransactionFlow.BillingSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController:ControllerBase
{
    private IAccountService _accountService;
    private ISessionService _sessionService;

    public CustomerController(IAccountService accountService, ISessionService sessionService)
    {
        _accountService = accountService;
        _sessionService = sessionService;
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

        var idempotencyResult = _sessionService.Get(key);
        if (idempotencyResult == null)
        {
            var result = _accountService.UpdateCustomer(customerId, customer);
            if (result.IsFailed)
            {
                return BadRequest(result.Reasons);
            }

            _sessionService.Set(key);
            return Ok();
        }

        return Ok();
    }
    
    [Route(nameof(CreateCustomerAsync))]
    [HttpPost]
    public async Task<IActionResult> CreateCustomerAsync([FromBody] CustomerDto customer)
    {
        var key = Request.Headers["Idempotency-key"].ToString();

        var idempotencyResult = _sessionService.Get(key);
        if (idempotencyResult == null)
        {
            var result = await _accountService.CreateCustomerAsync(customer);
            if (result.IsFailed)
            {
                return BadRequest(result.Reasons);
            }

            _sessionService.Set(key);
            return Ok("created!");
        }

        return Ok("Already created!");
    }
    
    [Route(nameof(DeleteCustomerAsync))]
    [HttpPost]
    public async Task<IActionResult> DeleteCustomerAsync([FromBody] int customerId)
    {
        var key = Request.Headers["Idempotency-key"].ToString();

        var idempotencyResult = _sessionService.Get(key);
        if (idempotencyResult == null)
        {
            var result = await _accountService.DeleteCustomerAsync(customerId);
            if (result.IsFailed)
            {
                return BadRequest(result.Reasons);
            }

            _sessionService.Set(key);
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