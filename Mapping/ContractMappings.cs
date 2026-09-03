using InsuranceDomain;
using InsuranceDomain.Services;
using InsureApi.Contracts;

namespace InsureApi.Mapping;

public static class ContractMappings
{
    public static CustomerResponse ToResponse(this Customer customer) => new(
        customer.CustomerId.ToString(),
        customer.FirstName,
        customer.LastName,
        customer.DateOfBirth);

    public static AddressResponse ToResponse(this Address address) => new(
        address.AddressId.ToString(),
        address.AddressLine1,
        address.AddressLine2,
        address.AddressLine3,
        address.Postcode);

    public static PolicyResponse ToResponse(this Policy policy) => new(
        policy.UniqueReference,
        policy.StartDate,
        policy.EndDate,
        policy.Amount,
        policy.HasClaims,
        policy.AutoRenew,
        policy.CustomerIds.Select(id => id.ToString()).ToArray(),
        policy.AddressId.ToString());

    public static PaymentResponse ToResponse(this Payment payment) => new(
        payment.PaymentReference,
        payment.Type,
        payment.Amount);

    public static RefundResponse ToResponse(this Refund refund) => new(
        refund.RefundReference,
        refund.Type,
        refund.Amount);

    public static CancellationQuoteResponse ToResponse(this CancellationQuote quote) => new(
        quote.CancellationDate,
        quote.CancellationCost,
        quote.RefundAmount);
}
