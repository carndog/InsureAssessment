namespace InsuranceDomain;

public abstract class Policy
{
    private readonly Guid[] _customerIds;
    private readonly List<Payment> _payments = [];
    private readonly List<Refund> _refunds = [];

    protected Policy(
        string uniqueReference,
        DateOnly startDate,
        DateOnly endDate,
        decimal amount,
        bool autoRenew,
        IEnumerable<Guid> customerIds,
        Guid addressId,
        Payment initialPayment)
    {
        if (string.IsNullOrWhiteSpace(uniqueReference))
            throw new DomainRuleException("UniqueReference is required.");
        if (endDate != startDate.AddYears(1))
            throw new DomainRuleException("A Policy must be exactly one year in length.");
        if (amount < 0)
            throw new DomainRuleException("Policy Amount cannot be negative.");
        if (addressId == Guid.Empty)
            throw new DomainRuleException("AddressId is required.");

        _customerIds = customerIds.ToArray();
        if (_customerIds.Length is < 1 or > 3)
            throw new DomainRuleException("A Policy must have between 1 and 3 Customers.");
        if (_customerIds.Distinct().Count() != _customerIds.Length)
            throw new DomainRuleException("A Policy cannot contain the same Customer more than once.");

        UniqueReference = uniqueReference.Trim();
        StartDate = startDate;
        EndDate = endDate;
        Amount = amount;
        AutoRenew = autoRenew;
        AddressId = addressId;
        _payments.Add(initialPayment);
    }

    public string UniqueReference { get; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public decimal Amount { get; private set; }
    public bool HasClaims { get; private set; }
    public bool AutoRenew { get; }
    public IReadOnlyList<Guid> CustomerIds => _customerIds;
    public Guid AddressId { get; }
    public IReadOnlyList<Payment> Payments => _payments;
    public IReadOnlyList<Refund> Refunds => _refunds;
    public DateOnly? CancellationDate { get; private set; }
    public bool IsCancelled => CancellationDate.HasValue;

    public void RegisterClaim() => HasClaims = true;

    public decimal CalculateRefundAmount(DateOnly cancellationDate)
    {
        if (cancellationDate > EndDate)
            throw new DomainRuleException("A Policy cannot be cancelled after its EndDate.");

        if (cancellationDate <= StartDate.AddDays(14))
            return Amount;

        int totalDays = EndDate.DayNumber - StartDate.DayNumber;
        int unusedDays = EndDate.DayNumber - cancellationDate.DayNumber;
        decimal refund = Amount * unusedDays / totalDays;

        return decimal.Round(refund, 2, MidpointRounding.AwayFromZero);
    }

    public Refund Cancel(DateOnly cancellationDate, string refundReference)
    {
        if (IsCancelled)
            throw new DomainRuleException("The Policy has already been cancelled.");

        Refund refund = new Refund(
            refundReference,
            _payments[0].Type,
            CalculateRefundAmount(cancellationDate));

        CancellationDate = cancellationDate;
        _refunds.Add(refund);
        return refund;
    }

    public Payment? Renew(
        DateOnly today,
        DateOnly newStartDate,
        DateOnly newEndDate,
        decimal newAmount,
        string paymentReference)
    {
        if (IsCancelled)
            throw new DomainRuleException("A cancelled Policy cannot be renewed.");
        if (today < EndDate.AddDays(-30))
            throw new DomainRuleException("A Policy cannot be renewed more than 30 days before its EndDate.");
        if (today > EndDate)
            throw new DomainRuleException("A Policy cannot be renewed after its EndDate.");
        if (newEndDate != newStartDate.AddYears(1))
            throw new DomainRuleException("A renewed Policy must be exactly one year in length.");
        if (newAmount < 0)
            throw new DomainRuleException("Policy Amount cannot be negative.");

        StartDate = newStartDate;
        EndDate = newEndDate;
        Amount = newAmount;

        if (!AutoRenew)
            return null;

        Payment payment = new Payment(paymentReference, _payments[^1].Type, newAmount);
        _payments.Add(payment);
        return payment;
    }
}
