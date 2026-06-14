namespace RevenueRecognitionSystem.Core.DTOs.SubscriptionDTOs;

public record SubscriptionResponse(
    int Id,
    int CustomerId,
    int SoftwareId,
    string Name,
    decimal PricePerPeriod,
    int RenewalPeriodMonths,
    DateTime ValidUntil,
    string Status);
