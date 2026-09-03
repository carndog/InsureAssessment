using InsuranceDomain;

namespace InsureApi.Contracts;

public sealed record PaymentRequest(
    string PaymentReference,
    PaymentMethod Type,
    decimal Amount);
