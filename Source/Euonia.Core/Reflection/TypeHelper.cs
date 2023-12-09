using System.ComponentModel;

namespace Nerosoft.Euonia.Reflection;

/// <summary>
/// 
/// </summary>
public class TypeHelper
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="desiredType"></param>
	/// <param name="valueType"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	public static object CoerceValue(Type desiredType, Type valueType, object value)
	{
		if (desiredType.IsAssignableFrom(valueType))
		{
			// types match, just return value
			return value;
		}

		if (desiredType.IsGenericType)
		{
			if (desiredType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				if (value == null)
				{
					return null;
				}

				if (valueType == typeof(string) && Convert.ToString(value) == string.Empty)
				{
					return null;
				}
			}
		}

		desiredType = desiredType.GetPropertyType();

		if (desiredType.IsEnum)
		{
			if ((value as byte?).HasValue)
			{
				return Enum.Parse(desiredType, ((byte?)value).Value.ToString());
			}

			if ((value as short?).HasValue)
			{
				return Enum.Parse(desiredType, ((short?)value).Value.ToString());
			}

			if ((value as int?).HasValue)
			{
				return Enum.Parse(desiredType, ((int?)value).Value.ToString());
			}

			if ((value as long?).HasValue)
			{
				return Enum.Parse(desiredType, ((long?)value).Value.ToString());
			}
		}

		if (desiredType.IsEnum && (valueType == typeof(string) || Enum.GetUnderlyingType(desiredType) == valueType))
		{
			return Enum.Parse(desiredType, value.ToString() ?? string.Empty);
		}

		if ((desiredType.IsPrimitive || desiredType == typeof(decimal)) && valueType == typeof(string) && string.IsNullOrEmpty((string)value))
		{
			value = 0;
		}

		try
		{
			if (desiredType == typeof(string) && value != null)
			{
				return value.ToString();
			}

			return Convert.ChangeType(value, desiredType);
		}
		catch
		{
			var converter = TypeDescriptor.GetConverter(desiredType);
			if (valueType != null)
			{
				var cnv1 = TypeDescriptor.GetConverter(valueType);
				if (converter.CanConvertFrom(valueType))
				{
					return converter.ConvertFrom(value);
				}

				if (cnv1.CanConvertTo(desiredType))
				{
					return cnv1.ConvertTo(value!, desiredType);
				}
			}

			throw;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="valueType"></param>
	/// <param name="value"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public static T CoerceValue<T>(Type valueType, object value)
	{
		return (T)CoerceValue(typeof(T), valueType, value);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TOutput"></typeparam>
	/// <typeparam name="TInput"></typeparam>
	/// <param name="value"></param>
	/// <returns></returns>
	public static TOutput CoerceValue<TOutput, TInput>(TInput value)
	{
		return (TOutput)CoerceValue(typeof(TOutput), typeof(TInput), value);
	}
}