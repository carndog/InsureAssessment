using InsuranceDomain.DataLayer;

namespace InsuranceDomain.Services;

public sealed class RetrievePolicyService(InsuranceStore store)
{
    public Policy GetPolicy(string reference) => store.GetPolicy(reference);
}
