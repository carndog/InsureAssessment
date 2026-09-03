namespace InsuranceDomain;

public sealed class InsuranceStore
{
    private readonly Dictionary<Guid, Customer> _customers = [];
    private readonly Dictionary<Guid, Address> _addresses = [];
    private readonly Dictionary<string, Policy> _policies =
        new(StringComparer.OrdinalIgnoreCase);

    public void Add(Customer customer) => _customers.Add(customer.CustomerId, customer);
    public void Add(Address address) => _addresses.Add(address.AddressId, address);
    public void Add(Policy policy) => _policies.Add(policy.UniqueReference, policy);

    public Customer GetCustomer(Guid id) =>
        _customers.TryGetValue(id, out Customer? customer)
            ? customer
            : throw new EntityNotFoundException($"Customer '{id}' was not found.");

    public Address GetAddress(Guid id) =>
        _addresses.TryGetValue(id, out Address? address)
            ? address
            : throw new EntityNotFoundException($"Address '{id}' was not found.");

    public Policy GetPolicy(string reference) =>
        _policies.TryGetValue(reference, out Policy? policy)
            ? policy
            : throw new EntityNotFoundException($"Policy '{reference}' was not found.");

    public IReadOnlyCollection<Policy> GetPolicies(Guid customerId) =>
        _policies.Values.Where(policy => policy.CustomerIds.Contains(customerId)).ToArray();
}
