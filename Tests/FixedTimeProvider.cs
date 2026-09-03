namespace InsureApi.Tests;

public sealed class FixedTimeProvider(DateOnly fixedDate) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => fixedDate.ToDateTime(TimeOnly.MinValue).ToUniversalTime();
}
