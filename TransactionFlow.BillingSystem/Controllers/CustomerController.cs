using Microsoft.AspNetCore.Mvc;
using TransactionFlow.BillingSystem.Models.Dtos;
using TransactionFlow.BillingSystem.Services.Abstraction;

namespace TransactionFlow.BillingSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController:ControllerBase
{
    private IAccountService _accountService;

    public CustomerController(IAccountService accountService)
    {
        _accountService = accountService;
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

        return Ok();
    }
    
    [Route(nameof(UpdateCustomer))]
    [HttpGet]
    public IActionResult UpdateCustomer(int customerId,CustomerDto customer)
    {
        var result = _accountService.UpdateCustomer(customerId, customer);
        if (result.IsFailed)
        {
            return BadRequest(result.Reasons);
        }

        return Ok();
    }
    
    [Route(nameof(CreateCustomerAsync))]
    [HttpPost]
    public async Task<IActionResult> CreateCustomerAsync(CustomerDto customer)
    {
        var result = await _accountService.CreateCustomerAsync(customer);
        if (result.IsFailed)
        {
            return BadRequest(result.Reasons);
        }

        return Ok();
    }
    
    [Route(nameof(DeleteCustomerAsync))]
    [HttpPost]
    public async Task<IActionResult> DeleteCustomerAsync(int customerId)
    {
        var result = await _accountService.DeleteCustomerAsync(customerId);
        if (result.IsFailed)
        {
            return BadRequest(result.Reasons);
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