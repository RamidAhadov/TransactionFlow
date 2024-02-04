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

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [Route("addCustomer")]
    [HttpPost]
    public IActionResult AddCustomer(Customer customer)
    {
        var result = _customerService.Add(customer);
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