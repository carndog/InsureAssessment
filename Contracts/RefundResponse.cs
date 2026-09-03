using InsuranceDomain;

namespace InsureApi.Contracts;

public sealed record RefundResponse(
    string RefundReference,
    PaymentMethod Type,
    decimal Amount);
