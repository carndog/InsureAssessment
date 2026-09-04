namespace InsuranceDomain;

public sealed class HouseholdPolicy : Policy
{
    public HouseholdPolicy(
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
