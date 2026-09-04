using InsuranceDomain;
using InsuranceDomain.Services;

namespace InsureApi.Tests;

public sealed class SellPolicyServiceTests
{
    private readonly InsuranceStore _store;
    private readonly CustomerService _customerService;
    private readonly AddressService _addressService;
    private readonly TimeProvider _timeProvider;
    private readonly SellPolicyService _service;

    public SellPolicyServiceTests()
    {
        _store = new InsuranceStore();
        _timeProvider = new FixedTimeProvider(new DateOnly(2024, 1, 1));
        _customerService = new CustomerService(_store, _timeProvider);
        _addressService = new AddressService(_store);
        _service = new SellPolicyService(_store, _timeProvider);
    }

    [Fact]
    public void ValidHouseholdPolicy_IsSoldAndStored()
    {
        DateOnly today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
        DateOnly startDate = today.AddDays(1);
        DateOnly endDate = startDate.AddYears(1);

        Guid customerId = Guid.NewGuid();
        Customer customer = _customerService.Create(customerId, "John", "Doe", new DateOnly(1990, 1, 1));

        Guid addressId = Guid.NewGuid();
        Address address = _addressService.Create(addressId, "123 Main St", null, null, "SW1A 1AA");

        SellPolicyDetails details = new SellPolicyDetails(
            startDate,
            endDate,
            500.00m,
            true,
            [customerId],
            addressId,
            "PAY-REF-001",
            PaymentMethod.Card,
            500.00m);

        Policy policy = _service.SellHousehold(details);

        Assert.NotEmpty(policy.UniqueReference);
        Assert.Equal(startDate, policy.StartDate);
        Assert.Equal(endDate, policy.EndDate);
        Assert.Equal(500.00m, policy.Amount);
        Assert.True(policy.AutoRenew);
        Assert.Single(policy.CustomerIds);
        Assert.Equal(addressId, policy.AddressId);
        Assert.Single(policy.Payments);
    }

    [Fact]
    public void ValidBuyToLetPolicy_IsSoldAndStored()
    {
        DateOnly today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
        DateOnly startDate = today.AddDays(1);
        DateOnly endDate = startDate.AddYears(1);

        Guid customerId = Guid.NewGuid();
        Customer customer = _customerService.Create(customerId, "John", "Doe", new DateOnly(1990, 1, 1));

        Guid addressId = Guid.NewGuid();
        Address address = _addressService.Create(addressId, "123 Main St", null, null, "SW1A 1AA");

        SellPolicyDetails details = new SellPolicyDetails(
            startDate,
            endDate,
            500.00m,
            true,
            [customerId],
            addressId,
            "PAY-REF-001",
            PaymentMethod.Card,
            500.00m);

        Policy policy = _service.SellBuyToLet(details);

        Assert.NotEmpty(policy.UniqueReference);
        Assert.Equal(startDate, policy.StartDate);
        Assert.Equal(endDate, policy.EndDate);
        Assert.Equal(500.00m, policy.Amount);
    }

    [Fact]
    public void UniqueReference_IsGeneratedAndTwoSalesReceiveDifferentReferences()
    {
        DateOnly today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
        DateOnly startDate = today.AddDays(1);
        DateOnly endDate = startDate.AddYears(1);

        Guid customerId = Guid.NewGuid();
        Customer customer = _customerService.Create(customerId, "John", "Doe", new DateOnly(1990, 1, 1));

        Guid addressId = Guid.NewGuid();
        Address address = _addressService.Create(addressId, "123 Main St", null, null, "SW1A 1AA");

        SellPolicyDetails details1 = new SellPolicyDetails(
            startDate,
            endDate,
            500.00m,
            true,
            [customerId],
            addressId,
            "PAY-REF-001",
            PaymentMethod.Card,
            500.00m);

        SellPolicyDetails details2 = new SellPolicyDetails(
            startDate,
            endDate,
            600.00m,
            true,
            [customerId],
            addressId,
            "PAY-REF-002",
            PaymentMethod.Card,
            600.00m);

        Policy policy1 = _service.SellHousehold(details1);
        Policy policy2 = _service.SellHousehold(details2);

        Assert.NotEqual(policy1.UniqueReference, policy2.UniqueReference);
    }

    [Fact]
    public void StartDate_MoreThan60DaysAhead_IsRejected()
    {
        DateOnly today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
        DateOnly startDate = today.AddDays(61);
        DateOnly endDate = startDate.AddYears(1);

        Guid customerId = Guid.NewGuid();
        Customer customer = _customerService.Create(customerId, "John", "Doe", new DateOnly(1990, 1, 1));

        Guid addressId = Guid.NewGuid();
        Address address = _addressService.Create(addressId, "123 Main St", null, null, "SW1A 1AA");

        SellPolicyDetails details = new SellPolicyDetails(
            startDate,
            endDate,
            500.00m,
            true,
            [customerId],
            addressId,
            "PAY-REF-001",
            PaymentMethod.Card,
            500.00m);

        Assert.Throws<DomainRuleException>(() => _service.SellHousehold(details));
    }

    [Fact]
    public void StartDate_InThePast_IsRejected()
    {
        DateOnly today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
        DateOnly startDate = today.AddDays(-1);
        DateOnly endDate = startDate.AddYears(1);

        Guid customerId = Guid.NewGuid();
        Customer customer = _customerService.Create(customerId, "John", "Doe", new DateOnly(1990, 1, 1));

        Guid addressId = Guid.NewGuid();
        Address address = _addressService.Create(addressId, "123 Main St", null, null, "SW1A 1AA");

        SellPolicyDetails details = new SellPolicyDetails(
            startDate,
            endDate,
            500.00m,
            true,
            [customerId],
            addressId,
            "PAY-REF-001",
            PaymentMethod.Card,
            500.00m);

        Assert.Throws<DomainRuleException>(() => _service.SellHousehold(details));
    }

    [Fact]
    public void EndDate_OtherThanStartDateAddYears1_IsRejected()
    {
        DateOnly today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
        DateOnly startDate = today.AddDays(1);
        DateOnly endDate = startDate.AddDays(100);

        Guid customerId = Guid.NewGuid();
        Customer customer = _customerService.Create(customerId, "John", "Doe", new DateOnly(1990, 1, 1));

        Guid addressId = Guid.NewGuid();
        Address address = _addressService.Create(addressId, "123 Main St", null, null, "SW1A 1AA");

        SellPolicyDetails details = new SellPolicyDetails(
            startDate,
            endDate,
            500.00m,
            true,
            [customerId],
            addressId,
            "PAY-REF-001",
            PaymentMethod.Card,
            500.00m);

        Assert.Throws<DomainRuleException>(() => _service.SellHousehold(details));
    }

    [Fact]
    public void NoCustomers_IsRejected()
    {
        DateOnly today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
        DateOnly startDate = today.AddDays(1);
        DateOnly endDate = startDate.AddYears(1);

        Guid addressId = Guid.NewGuid();
        Address address = _addressService.Create(addressId, "123 Main St", null, null, "SW1A 1AA");

        SellPolicyDetails details = new SellPolicyDetails(
            startDate,
            endDate,
            500.00m,
            true,
            [],
            addressId,
            "PAY-REF-001",
            PaymentMethod.Card,
            500.00m);

        Assert.Throws<DomainRuleException>(() => _service.SellHousehold(details));
    }

    [Fact]
    public void MoreThanThreeCustomers_IsRejected()
    {
        DateOnly today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
        DateOnly startDate = today.AddDays(1);
        DateOnly endDate = startDate.AddYears(1);

        Guid customerId1 = Guid.NewGuid();
        Guid customerId2 = Guid.NewGuid();
        Guid customerId3 = Guid.NewGuid();
        Guid customerId4 = Guid.NewGuid();

        _customerService.Create(customerId1, "John", "Doe", new DateOnly(1990, 1, 1));
        _customerService.Create(customerId2, "Jane", "Doe", new DateOnly(1991, 1, 1));
        _customerService.Create(customerId3, "Bob", "Smith", new DateOnly(1992, 1, 1));
        _customerService.Create(customerId4, "Alice", "Smith", new DateOnly(1993, 1, 1));

        Guid addressId = Guid.NewGuid();
        Address address = _addressService.Create(addressId, "123 Main St", null, null, "SW1A 1AA");

        SellPolicyDetails details = new SellPolicyDetails(
            startDate,
            endDate,
            500.00m,
            true,
            [customerId1, customerId2, customerId3, customerId4],
            addressId,
            "PAY-REF-001",
            PaymentMethod.Card,
            500.00m);

        Assert.Throws<DomainRuleException>(() => _service.SellHousehold(details));
    }

    [Fact]
    public void DuplicateCustomerIds_DoNotCircumventThe1To3Rule()
    {
        DateOnly today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
        DateOnly startDate = today.AddDays(1);
        DateOnly endDate = startDate.AddYears(1);

        Guid customerId = Guid.NewGuid();
        _customerService.Create(customerId, "John", "Doe", new DateOnly(1990, 1, 1));

        Guid addressId = Guid.NewGuid();
        Address address = _addressService.Create(addressId, "123 Main St", null, null, "SW1A 1AA");

        SellPolicyDetails details = new SellPolicyDetails(
            startDate,
            endDate,
            500.00m,
            true,
            [customerId, customerId],
            addressId,
            "PAY-REF-001",
            PaymentMethod.Card,
            500.00m);

        Assert.Throws<DomainRuleException>(() => _service.SellHousehold(details));
    }

    [Fact]
    public void AMissingCustomerId_IsRejected()
    {
        DateOnly today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
        DateOnly startDate = today.AddDays(1);
        DateOnly endDate = startDate.AddYears(1);

        Guid customerId = Guid.NewGuid();
        Guid addressId = Guid.NewGuid();
        Address address = _addressService.Create(addressId, "123 Main St", null, null, "SW1A 1AA");

        SellPolicyDetails details = new SellPolicyDetails(
            startDate,
            endDate,
            500.00m,
            true,
            [customerId],
            addressId,
            "PAY-REF-001",
            PaymentMethod.Card,
            500.00m);

        Assert.Throws<EntityNotFoundException>(() => _service.SellHousehold(details));
    }

    [Fact]
    public void AMissingAddressId_IsRejected()
    {
        DateOnly today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
        DateOnly startDate = today.AddDays(1);
        DateOnly endDate = startDate.AddYears(1);

        Guid customerId = Guid.NewGuid();
        _customerService.Create(customerId, "John", "Doe", new DateOnly(1990, 1, 1));

        Guid addressId = Guid.NewGuid();

        SellPolicyDetails details = new SellPolicyDetails(
            startDate,
            endDate,
            500.00m,
            true,
            [customerId],
            addressId,
            "PAY-REF-001",
            PaymentMethod.Card,
            500.00m);

        Assert.Throws<EntityNotFoundException>(() => _service.SellHousehold(details));
    }

    [Fact]
    public void ACustomerNotOver16OnStartDate_IsRejected()
    {
        DateOnly today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
        DateOnly startDate = today.AddDays(1);
        DateOnly endDate = startDate.AddYears(1);

        Guid customerId = Guid.NewGuid();
        DateOnly dateOfBirth = today.AddYears(-15);
        _customerService.Create(customerId, "John", "Doe", dateOfBirth);

        Guid addressId = Guid.NewGuid();
        Address address = _addressService.Create(addressId, "123 Main St", null, null, "SW1A 1AA");

        SellPolicyDetails details = new SellPolicyDetails(
            startDate,
            endDate,
            500.00m,
            true,
            [customerId],
            addressId,
            "PAY-REF-001",
            PaymentMethod.Card,
            500.00m);

        Assert.Throws<DomainRuleException>(() => _service.SellHousehold(details));
    }

    [Fact]
    public void AValidSale_CreatesExactlyOnePaymentWithTheSuppliedMethodAndAmount()
    {
        DateOnly today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
        DateOnly startDate = today.AddDays(1);
        DateOnly endDate = startDate.AddYears(1);

        Guid customerId = Guid.NewGuid();
        _customerService.Create(customerId, "John", "Doe", new DateOnly(1990, 1, 1));

        Guid addressId = Guid.NewGuid();
        Address address = _addressService.Create(addressId, "123 Main St", null, null, "SW1A 1AA");

        SellPolicyDetails details = new SellPolicyDetails(
            startDate,
            endDate,
            500.00m,
            true,
            [customerId],
            addressId,
            "PAY-REF-001",
            PaymentMethod.DirectDebit,
            500.00m);

        Policy policy = _service.SellHousehold(details);

        Assert.Single(policy.Payments);
        Assert.Equal("PAY-REF-001", policy.Payments[0].PaymentReference);
        Assert.Equal(PaymentMethod.DirectDebit, policy.Payments[0].Type);
        Assert.Equal(500.00m, policy.Payments[0].Amount);
    }
}
