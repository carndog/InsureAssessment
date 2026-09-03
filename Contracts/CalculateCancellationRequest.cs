namespace InsureApi.Contracts;

public sealed record CalculateCancellationRequest(
    DateOnly CancellationDate);
