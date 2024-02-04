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
            return BadRequest();
        }

        return Ok();
    }
}