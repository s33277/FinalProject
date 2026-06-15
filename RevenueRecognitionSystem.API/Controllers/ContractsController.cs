using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RevenueRecognitionSystem.Core.Abstractions;
using RevenueRecognitionSystem.Core.DTOs.ContractDTOs;

namespace RevenueRecognitionSystem.API.Controllers;

[ApiController]
[Authorize(Roles = "User,Admin")]
[Route("api/contracts")]
public class ContractsController : ControllerBase
{
    private readonly IContractService _contractService;

    public ContractsController(IContractService contractService)
    {
        _contractService = contractService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ContractResponse>>> GetAll()
    {
        return Ok(await _contractService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ContractResponse>> GetById(int id)
    {
        var contract = await _contractService.GetByIdAsync(id);

        return contract is null ? NotFound() : Ok(contract);
    }

    [HttpPost]
    public async Task<ActionResult<ContractResponse>> Create(CreateContractRequest request)
    {
        var contract = await _contractService.CreateAsync(request);

        if (contract is null)
        {
            return BadRequest("Contract could not be created. Check customer, software, and active product ownership rules.");
        }

        return CreatedAtAction(nameof(GetById), new { id = contract.Id }, contract);
    }

    [HttpPost("{id:int}/payments")]
    public async Task<ActionResult<ContractResponse>> AddPayment(int id, ContractPaymentRequest request)
    {
        var contract = await _contractService.AddPaymentAsync(id, request);

        return contract is null ? BadRequest("Payment could not be accepted.") : Ok(contract);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _contractService.DeleteAsync(id);

        return deleted ? NoContent() : NotFound();
    }
}
