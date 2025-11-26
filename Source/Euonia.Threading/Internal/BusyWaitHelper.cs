namespace Nerosoft.Euonia.Threading;

/// <summary>
/// Methods for waiting for a value to be available
/// </summary>
public static class BusyWaitHelper
{
    /// <summary>
    /// Waits asynchronously for a value to become available by repeatedly calling <paramref name="tryGetValue"/> until it returns a non-null result,
    /// </summary>
    /// <param name="state"></param>
    /// <param name="tryGetValue"></param>
    /// <param name="timeout"></param>
    /// <param name="minSleepTime"></param>
    /// <param name="maxSleepTime"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static async ValueTask<TResult> WaitAsync<TState, TResult>(
        TState state,
        Func<TState, CancellationToken, ValueTask<TResult>> tryGetValue, 
        TimeoutValue timeout,
        TimeoutValue minSleepTime,
        TimeoutValue maxSleepTime,
        CancellationToken cancellationToken)
        where TResult : class
    {
        Invariant.Require(minSleepTime.CompareTo(maxSleepTime) <= 0);
        Invariant.Require(!maxSleepTime.IsInfinite);

        var initialResult = await tryGetValue(state, cancellationToken).ConfigureAwait(false);
        if (initialResult != null || timeout.IsZero)
        {
            return initialResult;
        }

        using var _ = CreateMergedCancellationTokenSource(timeout, cancellationToken, out var mergedCancellationToken);

        var random = new Random(Guid.NewGuid().GetHashCode());
        var sleepRangeMillis = maxSleepTime.InMilliseconds - minSleepTime.InMilliseconds;
        while (true)
        {
            var sleepTime = minSleepTime.TimeSpan + TimeSpan.FromMilliseconds(random.NextDouble() * sleepRangeMillis);
            try
            {
                await TaskHelper.Delay(sleepTime, mergedCancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (IsTimedOut())
            {
                // if we time out while sleeping, always try one more time with just the regular token
                return await tryGetValue(state, cancellationToken).ConfigureAwait(false);
            }

            try
            {
                var result = await tryGetValue(state, mergedCancellationToken).ConfigureAwait(false);
                if (result != null) { return result; }
            }
            catch (OperationCanceledException) when (IsTimedOut())
            {
                return null;
            }
        }

        bool IsTimedOut() => 
            mergedCancellationToken.IsCancellationRequested && !cancellationToken.IsCancellationRequested;
    }

    private static IDisposable CreateMergedCancellationTokenSource(TimeoutValue timeout, CancellationToken cancellationToken, out CancellationToken mergedCancellationToken)
    {
        if (timeout.IsInfinite)
        {
            mergedCancellationToken = cancellationToken;
            return null;
        }

        if (!cancellationToken.CanBeCanceled)
        {
            var timeoutSource = new CancellationTokenSource(millisecondsDelay: timeout.InMilliseconds);
            mergedCancellationToken = timeoutSource.Token;
            return timeoutSource;
        }

        var mergedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        mergedSource.CancelAfter(timeout.InMilliseconds);
        mergedCancellationToken = mergedSource.Token;
        return mergedSource;
    }
}