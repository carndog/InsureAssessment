namespace InsureApi.Contracts;

public sealed record PolicyResponse(
    string UniqueReference,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal Amount,
    bool HasClaims,
    bool AutoRenew,
    IReadOnlyCollection<string> CustomerIds,
    string AddressId);
