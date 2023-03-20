namespace Nerosoft.Euonia.Domain;

internal static class Constants
{
    /// <summary>
    /// The event intent metadata key
    /// </summary>
    public const string EventIntentMetadataKey = "$nerosoft.euonia.event.intent";

    /// <summary>
    /// The event originator type key
    /// </summary>
    public const string EventOriginTypeKey = "$nerosoft.euonia.event.originator.type";

    /// <summary>
    /// The event originator identifier
    /// </summary>
    public const string EventOriginatorId = "$nerosoft.euonia.event.originator.id";

    /// <summary>
    /// The minimal sequence value.
    /// </summary>
    public const long MinimalSequence = -1L;

    /// <summary>
    /// The maximum sequence value.
    /// </summary>
    public const long MaximumSequence = long.MaxValue;
}