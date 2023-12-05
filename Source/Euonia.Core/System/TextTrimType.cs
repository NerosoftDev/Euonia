namespace System;

/// <summary>
/// The text trim type.
/// </summary>
public enum TextTrimType
{
	/// <summary>
	/// Represents no trim.
	/// </summary>
	None,

	/// <summary>
	/// Represents trim at the head of the text.
	/// </summary>
	Head,

	/// <summary>
	/// Represents trim at the tail of the text.
	/// </summary>
	Tail,

	/// <summary>
	/// Represents trim at both head and tail of the text.
	/// </summary>
	Both,

	/// <summary>
	/// Represents trim all the white spaces.
	/// </summary>
	All,
}
