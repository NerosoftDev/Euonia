using System.Runtime.CompilerServices;

namespace Nerosoft.Euonia.Bus.InMemory;

/// <summary>
/// An empty type representing a generic token with no specific value.
/// </summary>
internal readonly struct Unit : IEquatable<Unit>
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Unit other)
	{
		return true;
	}

	/// <inheritdoc/>
	public override bool Equals(object obj)
	{
		return obj is Unit;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
	{
		return 0;
	}
}