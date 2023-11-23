namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Represents the decorated message type should be enqueued.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class QueueAttribute : Attribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="QueueAttribute"/> class.
	/// </summary>
	/// <param name="name"></param>
	public QueueAttribute(string name)
	{
		Name = name;
	}

	/// <summary>
	/// Gets the name of the queue.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Gets or sets the priority of the message.
	/// </summary>
	public int Priority { get; set; }
}