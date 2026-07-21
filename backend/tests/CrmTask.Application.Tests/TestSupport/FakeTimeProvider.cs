namespace CrmTask.Application.Tests.TestSupport;

/// <summary>
/// A <see cref="TimeProvider"/> fixed to a given instant, so tests that involve
/// "today" (e.g. contract-status classification) are deterministic regardless
/// of when they actually run.
/// </summary>
public class FakeTimeProvider(DateTimeOffset now) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => now;
}
