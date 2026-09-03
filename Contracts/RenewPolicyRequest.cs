namespace InsureApi.Contracts;

public sealed record RenewPolicyRequest(
    DateOnly StartDate,
    DateOnly EndDate,
    decimal Amount);
