namespace RevenueRecognitionSystem.Core.DTOs.SubscriptionDTOs;

public record CreateSubscriptionRequest(
    int CustomerId,
    int SoftwareId,
    string Name,
    decimal PricePerPeriod,
    int RenewalPeriodMonths);

public record SubscriptionPaymentRequest(
    decimal Amount);
