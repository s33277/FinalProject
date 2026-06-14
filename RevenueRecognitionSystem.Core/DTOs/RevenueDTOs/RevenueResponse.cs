namespace RevenueRecognitionSystem.Core.DTOs.RevenueDTOs;

public record RevenueCalculationRequest(
    int? SoftwareId,
    string Currency = "PLN");

public record RevenueResponse(
    decimal Amount,
    string Currency);

public record RevenueSummaryResponse(
    decimal CurrentRevenue,
    decimal PredictedRevenue,
    string Currency);
