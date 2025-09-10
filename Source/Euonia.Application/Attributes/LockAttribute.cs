namespace Nerosoft.Euonia.Application;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true)]
public class LockAttribute : Attribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="LockAttribute"/> class.
	/// </summary>
	public LockAttribute()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="LockAttribute"/> class with the specified token.
	/// </summary>
	/// <param name="token">Specifies the lock token</param>
	public LockAttribute(string token)
		: this()
	{
		Token = token;
	}

	/// <summary>
	/// Gets the lock token.
	/// </summary>
	public string Token { get; }

	/// <summary>
	/// Gets or sets the lock timeout in milliseconds. Default is 30000 (30 seconds).
	/// </summary>
	public int Timeout { get; set; } = 30000;
}