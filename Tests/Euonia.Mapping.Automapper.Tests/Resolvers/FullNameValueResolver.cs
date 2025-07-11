using System.Linq.Expressions;
using AutoMapper;

namespace Nerosoft.Euonia.Mapping.Tests;

public class FullNameValueResolver<TSource, TDestination> : IValueResolver<TSource, TDestination, string>
{
	private readonly StringHelper _helper;

	public FullNameValueResolver(StringHelper helper)
	{
		_helper = helper;
	}

	public string Resolve(TSource source, TDestination destination, string destMember, ResolutionContext context)
	{
		var firstName = GetValue<string>(source, "FirstName");
		var lastName = GetValue<string>(source, "LastName");
		return _helper.Combine(firstName, lastName);
	}

	private static TValue GetValue<TValue>(TSource source, string name)
	{
		var property = Expression.PropertyOrField(Expression.Constant(source), name);
		var lambda = Expression.Lambda<Func<TSource, TValue>>(property, Expression.Parameter(typeof(TSource), nameof(source))).Compile();
		return lambda(source);
	}
}