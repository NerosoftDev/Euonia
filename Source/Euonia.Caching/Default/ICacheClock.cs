namespace Nerosoft.Euonia.Caching;

/// <summary>
/// Provides the current Utc <see cref="DateTime" />, and time related method for cache management.
/// This service should be used whenever the current date and time are needed, instead of <seealso cref="DateTime" /> directly.
/// It also makes implementations more testable, as time can be mocked.
/// Implements the <see cref="IVolatileProvider" />
/// </summary>
/// <seealso cref="IVolatileProvider" />
public interface ICacheClock : IVolatileProvider
{
    /// <summary>
    /// Gets the current <see cref="DateTime" /> of the system, expressed in Utc
    /// </summary>
    /// <value>The UTC now.</value>
    DateTime UtcNow { get; }

    /// <summary>
    /// Provides a <see cref="IVolatileToken" /> instance which can be used to cache some information for a
    /// specific duration.
    /// </summary>
    /// <param name="duration">The duration that the token must be valid.</param>
    /// <returns>IVolatileToken.</returns>
    /// <example>
    /// This sample shows how to use the <see cref="When" /> method by returning the result of
    /// a method named LoadVotes(), which is computed every 10 minutes only.
    /// <code>
    /// _cacheManager.Get("votes",
    /// ctx =&gt; {
    /// ctx.Monitor(_clock.When(TimeSpan.FromMinutes(10)));
    /// return LoadVotes();
    /// });
    /// </code></example>
    IVolatileToken When(TimeSpan duration);

    /// <summary>
    /// Provides a <see cref="IVolatileToken" /> instance which can be used to cache some
    /// until a specific date and time.
    /// </summary>
    /// <param name="absoluteUtc">The date and time that the token must be valid until.</param>
    /// <returns>IVolatileToken.</returns>
    /// <example>
    /// This sample shows how to use the <see cref="WhenUtc" /> method by returning the result of
    /// a method named LoadVotes(), which is computed once, and no more until the end of the year.
    /// <code>
    /// var endOfYear = _clock.UtcNow;
    /// endOfYear.Month = 12;
    /// endOfYear.Day = 31;
    /// _cacheManager.Get("votes",
    /// ctx =&gt; {
    /// ctx.Monitor(_clock.WhenUtc(endOfYear));
    /// return LoadVotes();
    /// });
    /// </code></example>
    IVolatileToken WhenUtc(DateTime absoluteUtc);
}