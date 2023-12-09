using Nerosoft.Euonia.Reflection;

namespace Nerosoft.Euonia.Domain;

/// <summary>
/// The abstract class implements <see cref="IEvent"/>.
/// </summary>
public abstract class Event : IEvent
{
	private const string PROPERTY_ID = "nerosoft.euonia.internal.event.id";

	/// <summary>
	/// Initializes a new instance of the <see cref="Event"/> class.
	/// </summary>
	protected Event()
	{
		var type = GetType();
		EventIntent = type.Name;
		Properties[PROPERTY_ID] = Guid.NewGuid().ToString();
	}

	/// <summary>
	/// Gets the extended properties of command.
	/// </summary>
	public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

	/// <summary>
	/// Gets or sets the command property with specified name.
	/// </summary>
	/// <param name="name"></param>
	public string this[string name]
	{
		get => Properties.TryGetValue(name, out var value) ? value : default;
		set => Properties[name] = value;
	}

	/// <summary>
	/// Gets value of property <paramref name="name"/>.
	/// </summary>
	/// <param name="name"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public virtual T GetProperty<T>(string name)
	{
		return TypeHelper.CoerceValue<T, string>(this[name]);
	}

	/// <summary>
	/// Gets or sets the event identifier.
	/// </summary>
	public string EventId
	{
		get => this[PROPERTY_ID];
		set => this[PROPERTY_ID] = value;
	}

	/// <summary>
	/// Gets or sets the sequence of the current event.
	/// </summary>
	public long Sequence { get; set; } = DateTime.UtcNow.Ticks;

	/// <summary>
	/// Gets the intent of the event.
	/// </summary>
	/// <returns>The intent of the event.</returns>
	public virtual string EventIntent { get; set; }

	/// <summary>
	/// Gets the .NET CLR type of the originator of the event.
	/// </summary>
	/// <returns>The .NET CLR type of the originator of the event.</returns>
	public virtual string OriginatorType { get; set; }

	/// <summary>
	/// Gets the originator identifier.
	/// </summary>
	/// <returns>The originator identifier.</returns>
	public virtual string OriginatorId { get; set; }
}