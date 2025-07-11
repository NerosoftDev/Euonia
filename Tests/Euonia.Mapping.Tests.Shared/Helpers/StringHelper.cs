namespace Nerosoft.Euonia.Mapping.Tests;

public class StringHelper
{
	public string Combine(params string[] values)
	{
		return string.Join(" ", values);
	}
}