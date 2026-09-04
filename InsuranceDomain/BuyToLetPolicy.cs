namespace InsuranceDomain;

public sealed class BuyToLetPolicy : Policy
{
    public BuyToLetPolicy(
        string uniqueReference,
        DateOnly startDate,
        DateOnly endDate,
        decimal amount,
        bool autoRenew,
        IEnumerable<Guid> customerIds,
        Guid addressId,
        Payment initialPayment)
        : base(uniqueReference, startDate, endDate, amount, autoRenew, customerIds, addressId, initialPayment, false)
    {
    }
}
