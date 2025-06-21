using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Nerosoft.Euonia.Reflection;

public static partial class Extensions
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="enum"></param>
	/// <typeparam name="TEnum"></typeparam>
	/// <returns></returns>
	public static bool IsValid<TEnum>(this TEnum @enum)
		where TEnum : Enum
	{
		return Enum.IsDefined(typeof(TEnum), @enum);
	}

	/// <summary>
	/// Gets a attribute of <typeparamref name="T"/> on enum.
	/// </summary>
	/// <typeparam name="T">The type of attribute.</typeparam>
	/// <param name="enum"></param>
	/// <returns></returns>
	public static T GetAttribute<T>(this Enum @enum) where T : Attribute
	{
		return EnumHelper.GetCustomerAttribute<T>(@enum);
	}

	/// <summary>
	/// Gets the enum field description text.
	/// </summary>
	/// <param name="enum"></param>
	/// <remarks>The field should add a <see cref="DescriptionAttribute"/>.</remarks>
	/// <returns></returns>
	public static string GetDescription(this Enum @enum)
	{
		var attribute = @enum.GetAttribute<DescriptionAttribute>();
		return attribute?.Description ?? @enum.ToString();
	}

	/// <summary>
	/// Gets the enum field description text from a resource manager.
	/// </summary>
	/// <param name="enum"></param>
	/// <param name="resourceManager"></param>
	/// <param name="resourceCulture"></param>
	/// <returns></returns>
	/// <exception cref="NullReferenceException"></exception>
	public static string GetDescription(this Enum @enum, ResourceManager resourceManager, CultureInfo resourceCulture = null)
	{
		resourceCulture = resourceCulture ?? CultureInfo.CurrentCulture;

		var field = @enum.GetType().GetField(@enum.ToString());
		if (field == null)
		{
			throw new NullReferenceException($"Field ‘{@enum}’ not defined.");
		}

		var attribute = field.GetCustomAttribute<DescriptionAttribute>();
		var key = attribute?.Description ?? @enum.ToString();
		var value = resourceManager.GetString(key, resourceCulture);
		return value;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="value"></param>
	/// <typeparam name="TEnum"></typeparam>
	/// <returns></returns>
	public static IEnumerable<TEnum> GetFlags<TEnum>(this Enum value)
		where TEnum : Enum
	{
		// ReSharper disable once LoopCanBeConvertedToQuery
		foreach (Enum item in Enum.GetValues(value.GetType()))
		{
			if (value.HasFlag(item))
			{
				yield return (TEnum)item;
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	public static IEnumerable<Enum> GetFlags(this Enum value)
	{
		return GetFlags(value, Enum.GetValues(value.GetType()).Cast<Enum>().ToArray());
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	public static IEnumerable<Enum> GetIndividualFlags(this Enum value)
	{
		return GetFlags(value, GetFlagValues(value.GetType()).ToArray());
	}

	// ReSharper disable once SuggestBaseTypeForParameter
	private static IEnumerable<Enum> GetFlags(Enum value, Enum[] values)
	{
		var bits = System.Convert.ToUInt64(value);
		var results = new List<Enum>();
		for (var i = values.Length - 1; i >= 0; i--)
		{
			var mask = System.Convert.ToUInt64(values[i]);
			if (i == 0 && mask == 0L)
			{
				break;
			}

			if ((bits & mask) != mask)
			{
				continue;
			}

			results.Add(values[i]);
			bits -= mask;
		}

		if (bits != 0L)
		{
			return Enumerable.Empty<Enum>();
		}

		if (System.Convert.ToUInt64(value) != 0L)
		{
			return results.Reverse<Enum>();
		}

		if (bits == System.Convert.ToUInt64(value) && values.Length > 0 && System.Convert.ToUInt64(values[0]) == 0L)
		{
			return values.Take(1);
		}

		return Enumerable.Empty<Enum>();
	}

	private static IEnumerable<Enum> GetFlagValues(Type enumType)
	{
		ulong flag = 0x1;
		foreach (var value in Enum.GetValues(enumType).Cast<Enum>())
		{
			var bits = System.Convert.ToUInt64(value);
			if (bits == 0L)
			{
				continue; // skip the zero value
			}

			while (flag < bits)
			{
				flag <<= 1;
			}

			if (flag == bits)
			{
				yield return value;
			}
		}
	}

	/// <summary>
	/// Gets the abbreviated day name of <see cref="DayOfWeek"/> in current culture.
	/// </summary>
	/// <param name="dayOfWeek"></param>
	/// <returns></returns>
	public static string AbbreviatedDayName(this DayOfWeek dayOfWeek)
	{
		return dayOfWeek.AbbreviatedDayName(CultureInfo.CurrentCulture);
	}

	/// <summary>
	/// Gets the abbreviated day name of <see cref="DayOfWeek"/> in specified culture.
	/// </summary>
	/// <param name="dayOfWeek"></param>
	/// <param name="cultureInfo"></param>
	/// <returns></returns>
	public static string AbbreviatedDayName(this DayOfWeek dayOfWeek, CultureInfo cultureInfo)
	{
		return cultureInfo.DateTimeFormat.AbbreviatedDayNames[(int)dayOfWeek];
	}
}