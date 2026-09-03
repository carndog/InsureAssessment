namespace InsuranceDomain;

public sealed class Payment
{
    public Payment(string paymentReference, PaymentMethod type, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(paymentReference))
            throw new DomainRuleException("PaymentReference is required.");
        if (amount < 0)
            throw new DomainRuleException("Payment Amount cannot be negative.");

        PaymentReference = paymentReference.Trim();
        Type = type;
        Amount = amount;
    }

    public string PaymentReference { get; }
    public PaymentMethod Type { get; }
    public decimal Amount { get; }
}
