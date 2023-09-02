using AutoMapper;
using Newtonsoft.Json;

namespace Nerosoft.Euonia.Mapping;

/// <summary>
/// The converter used to convert json <see cref="string"/> to <see cref="T"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public class JsonValueConverter<T> : IValueConverter<string, T>
{
	/// <inheritdoc />
	public T Convert(string sourceMember, ResolutionContext context)
    {
        if (string.IsNullOrEmpty(sourceMember))
        {
            return default;
        }

        try
        {
            return JsonConvert.DeserializeObject<T>(sourceMember);
        }
        catch (Exception)
        {
            return default;
        }
    }
}