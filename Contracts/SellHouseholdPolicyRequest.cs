namespace InsureApi.Contracts;

public sealed record SellHouseholdPolicyRequest(
    DateOnly StartDate,
    DateOnly EndDate,
    decimal Amount,
    bool AutoRenew,
    IReadOnlyCollection<Guid> CustomerIds,
    Guid AddressId,
    PaymentRequest Payment);
