using InsuranceDomain.Exceptions;

namespace InsuranceDomain;

public sealed class Refund
{
    public Refund(string refundReference, PaymentMethod type, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(refundReference))
            throw new DomainRuleException("RefundReference is required.");
        if (amount < 0)
            throw new DomainRuleException("Refund Amount cannot be negative.");

        RefundReference = refundReference.Trim();
        Type = type;
        Amount = amount;
    }

    public string RefundReference { get; }
    public PaymentMethod Type { get; }
    public decimal Amount { get; }
}
