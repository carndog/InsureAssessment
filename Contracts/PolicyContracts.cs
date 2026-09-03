using InsuranceDomain;

namespace InsureApi.Contracts;

public sealed record PaymentRequest(
    string PaymentReference,
    PaymentMethod Type,
    decimal Amount);

public sealed record PaymentResponse(
    string PaymentReference,
    PaymentMethod Type,
    decimal Amount);

public sealed record RefundResponse(
    string RefundReference,
    PaymentMethod Type,
    decimal Amount);

public sealed record SellHouseholdPolicyRequest(
    DateOnly StartDate,
    DateOnly EndDate,
    decimal Amount,
    bool AutoRenew,
    IReadOnlyCollection<string> CustomerIds,
    string AddressId,
    PaymentRequest Payment);

public sealed record SellBuyToLetPolicyRequest(
    DateOnly StartDate,
    DateOnly EndDate,
    decimal Amount,
    bool AutoRenew,
    IReadOnlyCollection<string> CustomerIds,
    string AddressId,
    PaymentRequest Payment);

public sealed record PolicyResponse(
    string UniqueReference,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal Amount,
    bool HasClaims,
    bool AutoRenew,
    IReadOnlyCollection<string> CustomerIds,
    string AddressId);

public sealed record CalculateCancellationRequest(
    DateOnly CancellationDate);

public sealed record CancellationQuoteResponse(
    DateOnly CancellationDate,
    decimal CancellationCost,
    decimal RefundAmount);

public sealed record CancelPolicyRequest(
    DateOnly CancellationDate);

public sealed record RenewPolicyRequest(
    DateOnly StartDate,
    DateOnly EndDate,
    decimal Amount);
