namespace InsuranceDomain.DataLayer;

public sealed class DataSeeder(InsuranceStore store)
{
    public void SeedDevelopmentData(TimeProvider timeProvider)
    {
        DateOnly today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        DateOnly endDate = today.AddDays(30);
        DateOnly startDate = endDate.AddYears(-1);

        Guid customerId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        Guid addressId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        if (!store.GetCustomers().ContainsKey(customerId))
        {
            Customer customer = new Customer(customerId, "John", "Doe", new DateOnly(1980, 1, 1));
            store.Add(customer);
        }

        if (!store.GetAddresses().ContainsKey(addressId))
        {
            Address address = new Address(addressId, "123 Main St", null, null, "AB1 2CD");
            store.Add(address);
        }

        string policyReference = "POL-RENEW-DEMO";
        if (!store.GetPolicies().ContainsKey(policyReference))
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
            store.Add(policy);
        }
    }
}
