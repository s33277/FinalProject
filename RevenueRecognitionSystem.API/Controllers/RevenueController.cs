using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RevenueRecognitionSystem.Core.Abstractions;
using RevenueRecognitionSystem.Core.DTOs.RevenueDTOs;

namespace RevenueRecognitionSystem.API.Controllers;

[ApiController]
[Authorize(Roles = "User,Admin")]
[Route("api/revenue")]
public class RevenueController : ControllerBase
{
    private readonly IRevenueService _revenueService;

    public RevenueController(IRevenueService revenueService)
    {
        _revenueService = revenueService;
    }

    [HttpGet("current")]
    public async Task<ActionResult<RevenueResponse>> GetCurrent([FromQuery] int? softwareId, [FromQuery] string currency = "PLN")
    {
        var revenue = await _revenueService.GetCurrentRevenueAsync(new RevenueCalculationRequest(softwareId, currency));

        return revenue is null ? BadRequest("Unsupported currency.") : Ok(revenue);
    }

    [HttpGet("predicted")]
    public async Task<ActionResult<RevenueResponse>> GetPredicted([FromQuery] int? softwareId, [FromQuery] string currency = "PLN")
    {
        var revenue = await _revenueService.GetPredictedRevenueAsync(new RevenueCalculationRequest(softwareId, currency));

        return revenue is null ? BadRequest("Unsupported currency.") : Ok(revenue);
    }

    [HttpGet("summary")]
    public async Task<ActionResult<RevenueSummaryResponse>> GetSummary([FromQuery] int? softwareId, [FromQuery] string currency = "PLN")
    {
        var revenue = await _revenueService.GetRevenueSummaryAsync(new RevenueCalculationRequest(softwareId, currency));

        return revenue is null ? BadRequest("Unsupported currency.") : Ok(revenue);
    }
}
