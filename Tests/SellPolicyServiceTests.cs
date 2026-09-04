using InsuranceDomain;
using InsuranceDomain.DataLayer;
using InsuranceDomain.Exceptions;
using InsuranceDomain.Services;

namespace InsureApi.Tests;

public sealed class SellPolicyServiceTests
{
    private readonly CustomerService _customerService;
    private readonly AddressService _addressService;
    private readonly TimeProvider _timeProvider;
    private readonly SellPolicyService _service;

    public SellPolicyServiceTests()
    {
        InsuranceStore store = new InsuranceStore();
        _timeProvider = new FixedTimeProvider(new DateOnly(2024, 1, 1));
        _customerService = new CustomerService(store);
        _addressService = new AddressService(store);
        _service = new SellPolicyService(store, _timeProvider);
    }

    [Test]
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

        Assert.That(policy.UniqueReference, Is.Not.Empty);
        Assert.That(policy.StartDate, Is.EqualTo(startDate));
        Assert.That(policy.EndDate, Is.EqualTo(endDate));
        Assert.That(policy.Amount, Is.EqualTo(500.00m));
        Assert.That(policy.AutoRenew, Is.True);
        Assert.That(policy.CustomerIds.Count, Is.EqualTo(1));
        Assert.That(policy.AddressId, Is.EqualTo(addressId));
        Assert.That(policy.Payments.Count, Is.EqualTo(1));
    }

    [Test]
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

        Assert.That(policy.UniqueReference, Is.Not.Empty);
        Assert.That(policy.StartDate, Is.EqualTo(startDate));
        Assert.That(policy.EndDate, Is.EqualTo(endDate));
        Assert.That(policy.Amount, Is.EqualTo(500.00m));
    }

    [Test]
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

        Assert.That(policy1.UniqueReference, Is.Not.EqualTo(policy2.UniqueReference));
    }

    [Test]
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

    [Test]
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

    [Test]
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

    [Test]
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

    [Test]
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

    [Test]
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

    [Test]
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

    [Test]
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

    [Test]
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

    [Test]
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

        Assert.That(policy.Payments.Count, Is.EqualTo(1));
        Assert.That(policy.Payments[0].PaymentReference, Is.EqualTo("PAY-REF-001"));
        Assert.That(policy.Payments[0].Type, Is.EqualTo(PaymentMethod.DirectDebit));
        Assert.That(policy.Payments[0].Amount, Is.EqualTo(500.00m));
    }
}
