using AutoMapper;
using Newtonsoft.Json;

namespace Nerosoft.Euonia.Mapping;

public class JsonValueConverter<T> : IValueConverter<string, T>
{
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