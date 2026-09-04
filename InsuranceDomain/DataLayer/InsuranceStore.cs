using System.Collections.Concurrent;
using InsuranceDomain.Exceptions;

namespace InsuranceDomain.DataLayer;

public sealed class InsuranceStore
{
    private readonly ConcurrentDictionary<Guid, Customer> _customers = [];
    private readonly ConcurrentDictionary<Guid, Address> _addresses = [];
    private readonly ConcurrentDictionary<string, Policy> _policies =
        new(StringComparer.OrdinalIgnoreCase);

    public void Add(Customer customer) => _customers.TryAdd(customer.CustomerId, customer);
    public void Add(Address address) => _addresses.TryAdd(address.AddressId, address);
    public void Add(Policy policy) => _policies.TryAdd(policy.UniqueReference, policy);

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

    public Policy[] GetPolicies(Guid customerId) =>
        _policies.Values.Where(policy => policy.CustomerIds.Contains(customerId)).ToArray();

    public ConcurrentDictionary<Guid, Customer> GetCustomers() => _customers;
    public ConcurrentDictionary<Guid, Address> GetAddresses() => _addresses;
    public ConcurrentDictionary<string, Policy> GetPolicies() => _policies;
}
