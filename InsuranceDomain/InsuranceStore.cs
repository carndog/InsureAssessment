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

    public void SeedDevelopmentData(TimeProvider timeProvider)
    {
        DateOnly today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        DateOnly startDate = today.AddDays(-335);
        DateOnly endDate = today.AddDays(30);

        Guid customerId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        Guid addressId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        if (!_customers.ContainsKey(customerId))
        {
            Customer customer = new Customer(customerId, "John", "Doe", new DateOnly(1980, 1, 1));
            Add(customer);
        }

        if (!_addresses.ContainsKey(addressId))
        {
            Address address = new Address(addressId, "123 Main St", null, null, "AB1 2CD");
            Add(address);
        }

        string policyReference = "POL-RENEW-DEMO";
        if (!_policies.ContainsKey(policyReference))
        {
            Payment payment = new Payment("PAY-SEED-001", PaymentMethod.Card, 500.00m);
            HouseholdPolicy policy = new HouseholdPolicy(
                policyReference,
                startDate,
                endDate,
                500.00m,
                true,
                [customerId],
                addressId,
                payment);
            Add(policy);
        }
    }
}
