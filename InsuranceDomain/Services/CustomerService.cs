namespace InsuranceDomain.Services;

public sealed class CustomerService(InsuranceStore store, TimeProvider timeProvider)
{
    public Customer Create(Guid customerId, string firstName, string lastName, DateOnly dateOfBirth)
    {
        Customer customer = new Customer(customerId, firstName, lastName, dateOfBirth);
        store.Add(customer);
        return customer;
    }

    public Customer Get(Guid customerId) => store.GetCustomer(customerId);

    public Policy[] GetPolicies(Guid customerId, bool includeLapsed = true)
    {
        DateOnly today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        Policy[] policies = store.GetPolicies(customerId);

        if (!includeLapsed)
        {
            return policies.Where(p => !p.IsLapsed(today)).ToArray();
        }

        return policies;
    }
}
