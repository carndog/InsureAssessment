using InsuranceDomain.DataLayer;

namespace InsuranceDomain.Services;

public sealed class CancelPolicyService(InsuranceStore store)
{
    public Refund Create(string reference, DateOnly cancellationDate)
    {
        Policy policy = store.GetPolicy(reference);
        string refundReference = $"REF-{Guid.NewGuid():N}".ToUpperInvariant();
        return policy.Cancel(cancellationDate, refundReference);
    }
}
