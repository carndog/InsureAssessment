using InsuranceDomain;
using InsuranceDomain.DataLayer;
using InsuranceDomain.Exceptions;
using InsuranceDomain.Services;

namespace InsureApi.Tests;

public sealed class RenewPolicyServiceTests
{
    private readonly InsuranceStore _store;
    private readonly CustomerService _customerService;
    private readonly AddressService _addressService;

    public RenewPolicyServiceTests()
    {
        _store = new InsuranceStore();
        _customerService = new CustomerService(_store);
        _addressService = new AddressService(_store);
    }

    private Policy CreateTestPolicy(DateOnly startDate, bool autoRenew = true)
    {
        TimeProvider timeProvider = new FixedTimeProvider(startDate.AddDays(-1));
        SellPolicyService sellPolicyService = new SellPolicyService(_store, timeProvider);

        Guid customerId = Guid.NewGuid();
        _customerService.Create(customerId, "John", "Doe", new DateOnly(1990, 1, 1));

        Guid addressId = Guid.NewGuid();
        _addressService.Create(addressId, "123 Main St", null, null, "SW1A 1AA");

        SellPolicyDetails details = new SellPolicyDetails(
            startDate,
            startDate.AddYears(1),
            500.00m,
            autoRenew,
            [customerId],
            addressId,
            "PAY-REF-001",
            PaymentMethod.Card,
            500.00m);

        return sellPolicyService.SellHousehold(details);
    }

    private RenewPolicyService CreateRenewalService(DateOnly currentDate)
    {
        TimeProvider timeProvider = new FixedTimeProvider(currentDate);
        return new RenewPolicyService(_store, timeProvider);
    }

    [Test]
    public void Renewal_Within30DaysOfEndDate_Succeeds()
    {
        DateOnly startDate = new DateOnly(2024, 1, 1);
        Policy policy = CreateTestPolicy(startDate, true);

        DateOnly newStartDate = startDate.AddYears(1);
        DateOnly newEndDate = newStartDate.AddYears(1);
        DateOnly currentDate = startDate.AddDays(340);

        RenewalResult result = CreateRenewalService(currentDate).Create(policy.UniqueReference, newStartDate, newEndDate, 600.00m);

        Assert.That(result.Policy.StartDate, Is.EqualTo(newStartDate));
        Assert.That(result.Policy.EndDate, Is.EqualTo(newEndDate));
        Assert.That(result.Policy.Amount, Is.EqualTo(600.00m));
    }

    [Test]
    public void Renewal_MoreThan30DaysBeforeEndDate_IsRejected()
    {
        DateOnly startDate = new DateOnly(2024, 1, 1);
        Policy policy = CreateTestPolicy(startDate, true);

        DateOnly newStartDate = startDate.AddYears(1);
        DateOnly newEndDate = newStartDate.AddYears(1);
        DateOnly currentDate = startDate.AddDays(300);

        Assert.Throws<DomainRuleException>(() => CreateRenewalService(currentDate).Create(policy.UniqueReference, newStartDate, newEndDate, 600.00m));
    }

    [Test]
    public void Renewal_AfterEndDate_IsRejected()
    {
        DateOnly startDate = new DateOnly(2024, 1, 1);
        Policy policy = CreateTestPolicy(startDate, true);

        DateOnly newStartDate = startDate.AddYears(1);
        DateOnly newEndDate = newStartDate.AddYears(1);
        DateOnly currentDate = startDate.AddDays(400);

        Assert.Throws<DomainRuleException>(() => CreateRenewalService(currentDate).Create(policy.UniqueReference, newStartDate, newEndDate, 600.00m));
    }

    [Test]
    public void RenewalPeriod_OtherThanOneYear_IsRejected()
    {
        DateOnly startDate = new DateOnly(2024, 1, 1);
        Policy policy = CreateTestPolicy(startDate, true);

        DateOnly newStartDate = startDate.AddYears(1);
        DateOnly newEndDate = newStartDate.AddDays(100);
        DateOnly currentDate = startDate.AddDays(340);

        Assert.Throws<DomainRuleException>(() => CreateRenewalService(currentDate).Create(policy.UniqueReference, newStartDate, newEndDate, 600.00m));
    }

    [Test]
    public void RenewalOfACancelledPolicy_IsRejected()
    {
        DateOnly startDate = new DateOnly(2024, 1, 1);
        Policy policy = CreateTestPolicy(startDate, true);

        policy.Cancel(startDate.AddDays(10), "REF-001");

        DateOnly newStartDate = startDate.AddYears(1);
        DateOnly newEndDate = newStartDate.AddYears(1);
        DateOnly currentDate = startDate.AddDays(340);

        Assert.Throws<DomainRuleException>(() => CreateRenewalService(currentDate).Create(policy.UniqueReference, newStartDate, newEndDate, 600.00m));
    }

    [Test]
    public void AutoRenewTrue_CreatesAPayment()
    {
        DateOnly startDate = new DateOnly(2024, 1, 1);
        Policy policy = CreateTestPolicy(startDate, true);

        DateOnly newStartDate = startDate.AddYears(1);
        DateOnly newEndDate = newStartDate.AddYears(1);
        DateOnly currentDate = startDate.AddDays(340);

        RenewalResult result = CreateRenewalService(currentDate).Create(policy.UniqueReference, newStartDate, newEndDate, 600.00m);

        Assert.That(result.Payment, Is.Not.Null);
        Assert.That(result.Payment.Amount, Is.EqualTo(600.00m));
        Assert.That(policy.Payments.Count, Is.EqualTo(2));
    }

    [Test]
    public void AutoRenewFalse_DoesNotCreateAPayment()
    {
        DateOnly startDate = new DateOnly(2024, 1, 1);
        Policy policy = CreateTestPolicy(startDate, false);

        DateOnly newStartDate = startDate.AddYears(1);
        DateOnly newEndDate = newStartDate.AddYears(1);
        DateOnly currentDate = startDate.AddDays(340);

        RenewalResult result = CreateRenewalService(currentDate).Create(policy.UniqueReference, newStartDate, newEndDate, 600.00m);

        Assert.That(result.Payment, Is.Null);
        Assert.That(policy.Payments.Count, Is.EqualTo(1));
    }

    [Test]
    public void TheRenewalPayment_UsesTheMostRecentPaymentMethod()
    {
        DateOnly startDate = new DateOnly(2024, 1, 1);
        Policy policy = CreateTestPolicy(startDate, true);

        DateOnly newStartDate = startDate.AddYears(1);
        DateOnly newEndDate = newStartDate.AddYears(1);
        DateOnly currentDate = startDate.AddDays(340);

        RenewalResult result = CreateRenewalService(currentDate).Create(policy.UniqueReference, newStartDate, newEndDate, 600.00m);

        Assert.That(result.Payment, Is.Not.Null);
        Assert.That(result.Payment.Type, Is.EqualTo(policy.Payments[0].Type));
    }
}
