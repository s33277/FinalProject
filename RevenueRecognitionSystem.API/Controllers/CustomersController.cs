using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RevenueRecognitionSystem.Core.Abstractions;
using RevenueRecognitionSystem.Core.DTOs.CustomerDTOs;

namespace RevenueRecognitionSystem.API.Controllers;

[ApiController]
[Authorize(Roles = "User,Admin")]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CustomerResponse>>> GetAll()
    {
        return Ok(await _customerService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerResponse>> GetById(int id)
    {
        var customer = await _customerService.GetByIdAsync(id);

        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpPost("individuals")]
    public async Task<ActionResult<CustomerResponse>> CreateIndividual(CreateIndividualCustomerRequest request)
    {
        var customer = await _customerService.CreateIndividualAsync(request);

        if (customer is null)
        {
            return Conflict("A customer with this PESEL already exists.");
        }

        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }

    [HttpPost("companies")]
    public async Task<ActionResult<CustomerResponse>> CreateCompany(CreateCompanyCustomerRequest request)
    {
        var customer = await _customerService.CreateCompanyAsync(request);

        if (customer is null)
        {
            return Conflict("A customer with this KRS number already exists.");
        }

        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CustomerResponse>> Update(int id, UpdateCustomerRequest request)
    {
        var customer = await _customerService.UpdateAsync(id, request);

        return customer is null ? NotFound() : Ok(customer);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("individuals/{id:int}")]
    public async Task<IActionResult> DeleteIndividual(int id)
    {
        var deleted = await _customerService.DeleteIndividualAsync(id);

        return deleted ? NoContent() : NotFound();
    }
}
