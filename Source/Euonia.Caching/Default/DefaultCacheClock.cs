namespace Nerosoft.Euonia.Caching;

/// <summary>
/// Class Clock.
/// Implements the <see cref="ICacheClock" />
/// </summary>
/// <seealso cref="ICacheClock" />
public class DefaultCacheClock : ICacheClock
{
    /// <summary>
    /// Gets the current <see cref="DateTime" /> of the system, expressed in Utc
    /// </summary>
    /// <value>The UTC now.</value>
    public DateTime UtcNow => DateTime.UtcNow;

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
    /// ctx => {
    /// ctx.Monitor(_clock.When(TimeSpan.FromMinutes(10)));
    /// return LoadVotes();
    /// });
    /// </code></example>
    public IVolatileToken When(TimeSpan duration)
    {
        return new AbsoluteExpirationToken(this, duration);
    }

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
    public IVolatileToken WhenUtc(DateTime absoluteUtc)
    {
        return new AbsoluteExpirationToken(this, absoluteUtc);
    }

    private class AbsoluteExpirationToken : IVolatileToken
    {
        /// <summary>
        /// The clock
        /// </summary>
        private readonly ICacheClock _clock;

        /// <summary>
        /// The invalidate UTC
        /// </summary>
        private readonly DateTime _invalidateUtc;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbsoluteExpirationToken"/> class.
        /// </summary>
        /// <param name="clock">The clock.</param>
        /// <param name="invalidateUtc">The invalidate UTC.</param>
        public AbsoluteExpirationToken(ICacheClock clock, DateTime invalidateUtc)
        {
            _clock = clock;
            _invalidateUtc = invalidateUtc;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbsoluteExpirationToken"/> class.
        /// </summary>
        /// <param name="clock">The clock.</param>
        /// <param name="duration">The duration.</param>
        public AbsoluteExpirationToken(ICacheClock clock, TimeSpan duration)
        {
            _clock = clock;
            _invalidateUtc = _clock.UtcNow.Add(duration);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is current.
        /// </summary>
        /// <value><c>true</c> if this instance is current; otherwise, <c>false</c>.</value>
        public bool IsCurrent => _clock.UtcNow < _invalidateUtc;
    }
}