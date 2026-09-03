namespace InsuranceDomain.Services;

public sealed record RenewalResult(Policy Policy, Payment? Payment);

public sealed class RenewPolicyService(InsuranceStore store, TimeProvider timeProvider)
{
    public RenewalResult Execute(
        string reference,
        DateOnly startDate,
        DateOnly endDate,
        decimal amount)
    {
        Policy policy = store.GetPolicy(reference);
        DateOnly today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        string paymentReference = $"PAY-{Guid.NewGuid():N}".ToUpperInvariant();

        Payment? payment = policy.Renew(today, startDate, endDate, amount, paymentReference);
        return new RenewalResult(policy, payment);
    }
}
