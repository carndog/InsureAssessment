namespace InsuranceDomain.Services;

public sealed record CancellationQuote(
    DateOnly CancellationDate,
    decimal CancellationCost,
    decimal RefundAmount);
