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
    private ICustomerService _customerService;
    private IAccountService _accountService;

    public CustomerController(ICustomerService customerService, IAccountService accountService)
    {
        _customerService = customerService;
        _accountService = accountService;
    }

    [Route("CreateCustomer")]
    [HttpPost]
    public async Task<IActionResult> CreateCustomerAsync(Customer customer)
    {
        var result = await _customerService.AddAsync(customer);
        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        var accountResult = await _accountService.CreateAccountAsync(result.Data);
        if (!accountResult.Success)
        {
            return BadRequest(accountResult.Message);
        }

        return Ok();
    }

    [Route("DeleteCustomerAsync")]
    [HttpPost]
    public async Task<IActionResult> DeleteCustomerAsync(Customer customer)
    {
        var customerResult = await _customerService.DeleteCustomerAsync(customer);
        if (!customerResult.Success)
        {
            return BadRequest(customerResult.Message);
        }

        var result = await _accountService.DeleteAccountAsync(customer);
        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        return Ok();
    }
    
    [Route("DeleteCustomerAsync")]
    [HttpPost]
    public async Task<IActionResult> DeleteCustomerAsync(int customerId)
    {
        var customerResult = await _customerService.DeleteCustomerAsync(customerId);
        if (!customerResult.Success)
        {
            return BadRequest(customerResult.Message);
        }

        var result = await _accountService.DeleteAccountAsync(customerResult.Data);
        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        return Ok();
    }

    [Route("addCustomerAsync")]
    [HttpPost]
    public async Task<IActionResult> AddCustomerAsync(Customer customer)
    {
        var result = await _customerService.AddAsync(customer);
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
        var result = _customerService.GetCustomerById(id);
        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        return Ok(result.Data);
    }
}