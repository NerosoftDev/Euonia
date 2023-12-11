namespace System;

/// <summary>
/// Defines the <see cref="Empty" />.
/// </summary>
[Serializable]
public sealed class Empty : ISerializable
{
	private Empty()
	{
	}

	/// <summary>
	/// Defines the Value.
	/// </summary>
	public static readonly Empty Value = new();

	/// <inheritdoc />
	public override string ToString()
	{
		return string.Empty;
	}

	/// <inheritdoc />
	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
	}
}