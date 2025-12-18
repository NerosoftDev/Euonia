namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Configuration options for the message bus.
/// </summary>
public class MessageBusOptions
{
	/// <summary>
	/// The configuration section name for message bus settings.
	/// </summary>
	public const string ConfigurationSection = Constants.ConfigurationSection;

	/// <summary>
	/// The default transport name for the message bus.
	/// </summary>
	public string DefaultTransport { get; set; }

	/// <summary>
	/// Indicates whether to enable pipeline behaviors for message processing.
	/// </summary>
	public bool EnablePipelineBehaviors { get; set; } = true;
}
