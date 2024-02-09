using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TransactionFlow.Business.Abstraction;
using TransactionFlow.Entities.Concrete;
using TransactionFlow.Entities.Concrete.Dtos;

namespace TransactionFlow.BillingSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController:ControllerBase
{
    private ICustomerManager _customerManager;
    private IAccountManager _accountManager;

    public CustomerController(ICustomerManager customerManager, IAccountManager accountManager)
    {
        _customerManager = customerManager;
        _accountManager = accountManager;
    }

    [Route(nameof(CreateCustomerAsync))]
    [HttpPost]
    public async Task<IActionResult> CreateCustomerAsync(Customer customer)
    {
        var result = await _customerManager.AddAsync(customer);
        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        var accountResult = await _accountManager.CreateAccountAsync(result.Data);
        if (!accountResult.Success)
        {
            return BadRequest(accountResult.Message);
        }

        return Ok();
    }
    
    [Route(nameof(DeleteCustomerAsync))]
    [HttpPost]
    public async Task<IActionResult> DeleteCustomerAsync(int customerId)
    {
        var customerResult = await _customerManager.DeleteCustomerAsync(customerId);
        if (!customerResult.Success)
        {
            return BadRequest(customerResult.Message);
        }

        var result = await _accountManager.DeleteAccountAsync(customerResult.Data);
        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        return Ok();
    }


    [HttpGet]
    [Route(nameof(GetCustomerById))]
    public IActionResult GetCustomerById(int id)
    {
        var result = _customerManager.GetCustomerById(id);
        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        return Ok(result.Data);
    }
}