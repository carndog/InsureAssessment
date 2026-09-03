using InsuranceDomain;

namespace InsureApi.Contracts;

public sealed record PaymentResponse(
    string PaymentReference,
    PaymentMethod Type,
    decimal Amount);
