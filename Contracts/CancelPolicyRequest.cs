namespace InsureApi.Contracts;

public sealed record CancelPolicyRequest(
    DateOnly CancellationDate);
