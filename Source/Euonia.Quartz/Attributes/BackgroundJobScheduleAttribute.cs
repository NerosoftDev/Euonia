using Quartz;

namespace Nerosoft.Euonia.Quartz;

/// <summary>
/// The abstract base class for background job schedule attributes.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public abstract class BackgroundJobScheduleAttribute : Attribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="BackgroundJobScheduleAttribute"/> class.
	/// </summary>
	/// <param name="identity"></param>
	protected BackgroundJobScheduleAttribute(string identity)
	{
		Identity = identity;
	}

	/// <summary>
	/// Gets the trigger identity.
	/// </summary>
	public string Identity { get; }

	/// <summary>
	/// Gets or sets the start delay time in seconds.
	/// 0 means start immediately.
	/// </summary>
	public int Delay { get; set; }

	/// <summary>
	/// Gets or sets the trigger priority.
	/// </summary>
	public int Priority { get; set; }

	/// <summary>
	/// Gets or sets the trigger description.
	/// </summary>
	public string Description { get; set; }

	/// <summary>
	/// Configures the trigger builder.
	/// </summary>
	/// <returns></returns>
	public abstract IScheduleBuilder Configure();
}