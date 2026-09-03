namespace InsuranceDomain.Services;

public sealed record SellPolicyDetails(
    DateOnly StartDate,
    DateOnly EndDate,
    decimal Amount,
    bool AutoRenew,
    IReadOnlyCollection<Guid> CustomerIds,
    Guid AddressId,
    string PaymentReference,
    PaymentMethod PaymentType,
    decimal PaymentAmount);

public sealed class SellPolicyService(InsuranceStore store, TimeProvider timeProvider)
{
    public Policy SellHousehold(SellPolicyDetails details) =>
        Sell(details, (reference, payment) => new HouseholdPolicy(
            reference,
            details.StartDate,
            details.EndDate,
            details.Amount,
            details.AutoRenew,
            details.CustomerIds,
            details.AddressId,
            payment));

    public Policy SellBuyToLet(SellPolicyDetails details) =>
        Sell(details, (reference, payment) => new BuyToLetPolicy(
            reference,
            details.StartDate,
            details.EndDate,
            details.Amount,
            details.AutoRenew,
            details.CustomerIds,
            details.AddressId,
            payment));

    private Policy Sell(
        SellPolicyDetails details,
        Func<string, Payment, Policy> createPolicy)
    {
        DateOnly today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

        if (details.StartDate < today)
            throw new DomainRuleException("A Policy cannot start in the past.");
        if (details.StartDate > today.AddDays(60))
            throw new DomainRuleException("A Policy cannot be sold more than 60 days in advance.");

        store.GetAddress(details.AddressId);

        Customer[] customers = details.CustomerIds.Select(store.GetCustomer).ToArray();
        if (customers.Any(customer => !customer.IsOver16On(details.StartDate)))
            throw new DomainRuleException("All Customers must be over 16 on the Policy StartDate.");

        Payment payment = new Payment(
            details.PaymentReference,
            details.PaymentType,
            details.PaymentAmount);

        string reference = NewReference("POL");
        Policy policy = createPolicy(reference, payment);
        store.Add(policy);
        return policy;
    }

    private static string NewReference(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}".ToUpperInvariant();
}
