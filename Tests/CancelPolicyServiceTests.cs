using InsuranceDomain;
using InsuranceDomain.Services;

namespace InsureApi.Tests;

public sealed class CancelPolicyServiceTests
{
    private readonly InsuranceStore _store;
    private readonly CustomerService _customerService;
    private readonly AddressService _addressService;

    public CancelPolicyServiceTests()
    {
        _store = new InsuranceStore();
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

    [Fact]
    public void Cancellation_BeforeStartDate_ProducesAFullRefund()
    {
        DateOnly startDate = new DateOnly(2024, 1, 1);
        Policy policy = CreateTestPolicy(startDate);

        DateOnly cancellationDate = startDate.AddDays(-1);
        CancellationQuote quote = CreateCancellationCostService().Execute(policy.UniqueReference, cancellationDate);

        Assert.Equal(500.00m, quote.RefundAmount);
        Assert.Equal(0.00m, quote.CancellationCost);
    }

    [Fact]
    public void Cancellation_DuringThe14DayCoolingOffPeriod_ProducesAFullRefund()
    {
        DateOnly startDate = new DateOnly(2024, 1, 1);
        Policy policy = CreateTestPolicy(startDate);

        DateOnly cancellationDate = startDate.AddDays(14);
        CancellationQuote quote = CreateCancellationCostService().Execute(policy.UniqueReference, cancellationDate);

        Assert.Equal(500.00m, quote.RefundAmount);
        Assert.Equal(0.00m, quote.CancellationCost);
    }

    [Fact]
    public void Cancellation_AfterTheCoolingOffPeriod_ProducesAProRataRefund()
    {
        DateOnly startDate = new DateOnly(2023, 1, 1);
        Policy policy = CreateTestPolicy(startDate);

        DateOnly cancellationDate = startDate.AddDays(100);
        CancellationQuote quote = CreateCancellationCostService().Execute(policy.UniqueReference, cancellationDate);

        int totalDays = 365;
        int unusedDays = 265;
        decimal expectedRefund = decimal.Round(500.00m * unusedDays / totalDays, 2, MidpointRounding.AwayFromZero);

        Assert.Equal(expectedRefund, quote.RefundAmount);
        Assert.Equal(500.00m - expectedRefund, quote.CancellationCost);
    }

    [Fact]
    public void Refund_UsesTheSamePaymentMethodAsTheOriginalPayment()
    {
        DateOnly startDate = new DateOnly(2024, 1, 1);
        Policy policy = CreateTestPolicy(startDate);

        DateOnly cancellationDate = startDate.AddDays(5);
        Refund refund = CreateCancelPolicyService().Execute(policy.UniqueReference, cancellationDate);

        Assert.Equal(policy.Payments[0].Type, refund.Type);
    }

    [Fact]
    public void Cancellation_AfterEndDate_IsRejected()
    {
        DateOnly startDate = new DateOnly(2024, 1, 1);
        Policy policy = CreateTestPolicy(startDate);

        DateOnly cancellationDate = startDate.AddYears(1).AddDays(1);

        Assert.Throws<DomainRuleException>(() => CreateCancellationCostService().Execute(policy.UniqueReference, cancellationDate));
    }

    [Fact]
    public void CancellingAnAlreadyCancelledPolicy_IsRejected()
    {
        DateOnly startDate = new DateOnly(2024, 1, 1);
        Policy policy = CreateTestPolicy(startDate);

        DateOnly cancellationDate = startDate.AddDays(5);
        CreateCancelPolicyService().Execute(policy.UniqueReference, cancellationDate);

        Assert.Throws<DomainRuleException>(() => CreateCancelPolicyService().Execute(policy.UniqueReference, cancellationDate));
    }

    [Fact]
    public void CalculatingACancellationQuote_DoesNotCancelThePolicyOrAddARefund()
    {
        DateOnly startDate = new DateOnly(2024, 1, 1);
        Policy policy = CreateTestPolicy(startDate);

        DateOnly cancellationDate = startDate.AddDays(5);
        CreateCancellationCostService().Execute(policy.UniqueReference, cancellationDate);

        Assert.False(policy.IsCancelled);
        Assert.Empty(policy.Refunds);
    }
}
