namespace InsuranceDomain.Services;

public sealed class CancelPolicyService(InsuranceStore store)
{
    public Refund Execute(string reference, DateOnly cancellationDate)
    {
        Policy policy = store.GetPolicy(reference);
        string refundReference = $"REF-{Guid.NewGuid():N}".ToUpperInvariant();
        return policy.Cancel(cancellationDate, refundReference);
    }
}
