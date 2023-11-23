using System.Reflection;

namespace Nerosoft.Euonia.Bus;

public interface IMessageTypeCache
{
	IEnumerable<PropertyInfo> Properties { get; }
}