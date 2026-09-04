using InsuranceDomain;
using InsuranceDomain.DataLayer;
using InsuranceDomain.Exceptions;
using InsuranceDomain.Services;

namespace InsureApi.Tests;

public sealed class CancelPolicyServiceTests
{
    private readonly InsuranceStore _store;
    private readonly CustomerService _customerService;
    private readonly AddressService _addressService;
    private readonly TimeProvider _timeProvider;

    public CancelPolicyServiceTests()
    {
        _store = new InsuranceStore();
        _timeProvider = new FixedTimeProvider(new DateOnly(2024, 1, 1));
        _customerService = new CustomerService(_store);
        _addressService = new AddressService(_store);
    }

    private Policy CreateTestPolicy(DateOnly startDate)
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
            true,
            [customerId],
            addressId,
            "PAY-REF-001",
            PaymentMethod.Card,
            500.00m);

        return sellPolicyService.SellHousehold(details);
    }

    private CalculateCancellationCostService CreateCancellationCostService()
    {
        return new CalculateCancellationCostService(_store);
    }

    private CancelPolicyService CreateCancelPolicyService()
    {
        return new CancelPolicyService(_store);
    }

    [Test]
    public void Cancellation_BeforeStartDate_ProducesAFullRefund()
    {
        DateOnly startDate = new DateOnly(2024, 1, 1);
        Policy policy = CreateTestPolicy(startDate);

        DateOnly cancellationDate = startDate.AddDays(-1);
        CancellationQuote quote = CreateCancellationCostService().Calculate(policy.UniqueReference, cancellationDate);

        Assert.That(quote.RefundAmount, Is.EqualTo(500.00m));
        Assert.That(quote.CancellationCost, Is.EqualTo(0.00m));
    }

    [Test]
    public void Cancellation_DuringThe14DayCoolingOffPeriod_ProducesAFullRefund()
    {
        DateOnly startDate = new DateOnly(2024, 1, 1);
        Policy policy = CreateTestPolicy(startDate);

        DateOnly cancellationDate = startDate.AddDays(14);
        CancellationQuote quote = CreateCancellationCostService().Calculate(policy.UniqueReference, cancellationDate);

        Assert.That(quote.RefundAmount, Is.EqualTo(500.00m));
        Assert.That(quote.CancellationCost, Is.EqualTo(0.00m));
    }

    [Test]
    public void Cancellation_AfterTheCoolingOffPeriod_ProducesAProRataRefund()
    {
        DateOnly startDate = new DateOnly(2023, 1, 1);
        Policy policy = CreateTestPolicy(startDate);

        DateOnly cancellationDate = startDate.AddDays(100);
        CancellationQuote quote = CreateCancellationCostService().Calculate(policy.UniqueReference, cancellationDate);

        int totalDays = 365;
        int unusedDays = 265;
        decimal expectedRefund = decimal.Round(500.00m * unusedDays / totalDays, 2, MidpointRounding.AwayFromZero);

        Assert.That(expectedRefund, Is.EqualTo(quote.RefundAmount));
        Assert.That(500.00m - expectedRefund, Is.EqualTo(quote.CancellationCost));
    }

    [Test]
    public void Refund_UsesTheSamePaymentMethodAsTheOriginalPayment()
    {
        DateOnly startDate = new DateOnly(2024, 1, 1);
        Policy policy = CreateTestPolicy(startDate);

        DateOnly cancellationDate = startDate.AddDays(5);
        Refund refund = CreateCancelPolicyService().Create(policy.UniqueReference, cancellationDate);

        Assert.That(policy.Payments[0].Type, Is.EqualTo(refund.Type));
    }

    [Test]
    public void Cancellation_AfterEndDate_IsRejected()
    {
        DateOnly startDate = new DateOnly(2024, 1, 1);
        Policy policy = CreateTestPolicy(startDate);

        DateOnly cancellationDate = startDate.AddYears(1).AddDays(1);

        Assert.Throws<DomainRuleException>(() => CreateCancellationCostService().Calculate(policy.UniqueReference, cancellationDate));
    }

    [Test]
    public void CancellingAnAlreadyCancelledPolicy_IsRejected()
    {
        DateOnly startDate = new DateOnly(2024, 1, 1);
        Policy policy = CreateTestPolicy(startDate);

        DateOnly cancellationDate = startDate.AddDays(5);
        CreateCancelPolicyService().Create(policy.UniqueReference, cancellationDate);

        Assert.Throws<DomainRuleException>(() => CreateCancelPolicyService().Create(policy.UniqueReference, cancellationDate));
    }

    [Test]
    public void CalculatingACancellationQuote_DoesNotCancelThePolicyOrAddARefund()
    {
        DateOnly startDate = new DateOnly(2024, 1, 1);
        Policy policy = CreateTestPolicy(startDate);

        DateOnly cancellationDate = startDate.AddDays(5);
        CreateCancellationCostService().Calculate(policy.UniqueReference, cancellationDate);

        Assert.That(policy.IsCancelled, Is.False);
        Assert.That(policy.Refunds, Is.Empty);
    }
}
