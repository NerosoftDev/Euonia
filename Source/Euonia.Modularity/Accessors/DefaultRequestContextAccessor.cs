namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// The default implementation of <see cref="IRequestContextAccessor"/>.
/// </summary>
public class DefaultRequestContextAccessor
{
	private static readonly AsyncLocal<RequestContext> _context = new();

	/// <inheritdoc/>
	public RequestContext Context
	{
		get => _context.Value;
		set => _context.Value = value;
	}
}