namespace InsuranceDomain.Services;

public sealed record SellPolicyDetails(
    DateOnly StartDate,
    DateOnly EndDate,
    decimal Amount,
    bool AutoRenew,
    IReadOnlyCollection<Guid> CustomerIds,
    Guid AddressId,
    string PaymentReference,
    PaymentMethod PaymentType,
    decimal PaymentAmount);
