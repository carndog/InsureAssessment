namespace InsureApi.Contracts;

public sealed record SellHouseholdPolicyRequest(
    DateOnly StartDate,
    DateOnly EndDate,
    decimal Amount,
    bool AutoRenew,
    IReadOnlyCollection<string> CustomerIds,
    string AddressId,
    PaymentRequest Payment);
