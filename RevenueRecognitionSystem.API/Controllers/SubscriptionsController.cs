using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RevenueRecognitionSystem.Core.Abstractions;
using RevenueRecognitionSystem.Core.DTOs.SubscriptionDTOs;

namespace RevenueRecognitionSystem.API.Controllers;

[ApiController]
[Authorize(Roles = "User,Admin")]
[Route("api/subscriptions")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionsController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpGet]
    public async Task<ActionResult<List<SubscriptionResponse>>> GetAll()
    {
        return Ok(await _subscriptionService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SubscriptionResponse>> GetById(int id)
    {
        var subscription = await _subscriptionService.GetByIdAsync(id);

        return subscription is null ? NotFound() : Ok(subscription);
    }

    [HttpPost]
    public async Task<ActionResult<SubscriptionResponse>> Create(CreateSubscriptionRequest request)
    {
        var subscription = await _subscriptionService.CreateAsync(request);

        if (subscription is null)
        {
            return BadRequest("Subscription could not be created. Check customer, software, and active product ownership rules.");
        }

        return CreatedAtAction(nameof(GetById), new { id = subscription.Id }, subscription);
    }

    [HttpPost("{id:int}/payments")]
    public async Task<ActionResult<SubscriptionResponse>> AddPayment(int id, SubscriptionPaymentRequest request)
    {
        var subscription = await _subscriptionService.AddPaymentAsync(id, request);

        return subscription is null ? BadRequest("Subscription payment could not be accepted.") : Ok(subscription);
    }
}
