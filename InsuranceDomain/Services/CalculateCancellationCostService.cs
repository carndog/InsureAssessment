namespace InsuranceDomain.Services;

public sealed record CancellationQuote(
    DateOnly CancellationDate,
    decimal CancellationCost,
    decimal RefundAmount);

public sealed class CalculateCancellationCostService(InsuranceStore store)
{
    public CancellationQuote Execute(string reference, DateOnly cancellationDate)
    {
        Policy policy = store.GetPolicy(reference);
        decimal refundAmount = policy.CalculateRefundAmount(cancellationDate);
        decimal cancellationCost = policy.Amount - refundAmount;

        return new CancellationQuote(cancellationDate, cancellationCost, refundAmount);
    }
}
