namespace InsureApi.Contracts;

public sealed record CancellationQuoteResponse(
    DateOnly CancellationDate,
    decimal CancellationCost,
    decimal RefundAmount);
