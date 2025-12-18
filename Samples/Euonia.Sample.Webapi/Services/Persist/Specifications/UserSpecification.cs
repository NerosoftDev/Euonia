using Nerosoft.Euonia.Linq;
using Nerosoft.Euonia.Sample.Domain.Aggregates;

namespace Nerosoft.Euonia.Sample.Persist.Specifications;

/// <summary>
/// The user query specification.
/// </summary>
internal static class UserSpecification
{
	public static readonly Specification<User> All = new DirectSpecification<User>(t => t.Id != null);

	/// <summary>
	/// Id equals <paramref name="id"/>.
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public static Specification<User> IdEquals(string id)
	{
		return new DirectSpecification<User>(t => t.Id == id);
	}

	/// <summary>
	/// Id not equals <paramref name="id"/>.
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public static Specification<User> IdNotEquals(string id)
	{
		return new DirectSpecification<User>(t => t.Id != id);
	}

	/// <summary>
	/// Username equals <paramref name="username"/>.
	/// </summary>
	/// <param name="username"></param>
	/// <returns></returns>
	public static Specification<User> UsernameEquals(string username)
	{
		username = username.Normalize(TextCaseType.Lower);
		return new DirectSpecification<User>(t => t.Username == username);
	}

	/// <summary>
	/// Username contains <paramref name="username"/>.
	/// </summary>
	/// <param name="username"></param>
	/// <returns></returns>
	public static Specification<User> UsernameContains(string username)
	{
		username = username.Normalize(TextCaseType.Lower);
		return new DirectSpecification<User>(t => t.Username.Contains(username));
	}

	/// <summary>
	/// Nickname equals <paramref name="nickname"/>.
	/// </summary>
	/// <param name="nickname"></param>
	/// <returns></returns>
	public static Specification<User> NicknameContains(string nickname)
	{
		nickname = nickname.Normalize(TextCaseType.Lower);
		return new DirectSpecification<User>(t => t.Nickname.ToLower().Contains(nickname));
	}

	/// <summary>
	/// Email address equals <paramref name="email"/>
	/// </summary>
	/// <param name="email"></param>
	/// <returns></returns>
	public static Specification<User> EmailEquals(string email)
	{
		email = email.Normalize(TextCaseType.Lower);
		return new DirectSpecification<User>(t => t.Email == email);
	}

	public static Specification<User> EmailContains(string email)
	{
		email = email.Normalize(TextCaseType.Lower);
		return new DirectSpecification<User>(t => t.Email.Contains(email));
	}

	public static Specification<User> PhoneEquals(string phone)
	{
		phone = phone.Normalize(TextCaseType.Lower);
		return new DirectSpecification<User>(t => t.Phone == phone);
	}

	public static Specification<User> PhoneContains(string phone)
	{
		phone = phone.Normalize(TextCaseType.Lower);
		return new DirectSpecification<User>(t => t.Phone.Contains(phone));
	}

	public static Specification<User> Matches(string keyword)
	{
		ISpecification<User>[] specifications =
		[
			UsernameContains(keyword),
			NicknameContains(keyword),
			EmailContains(keyword),
			PhoneContains(keyword)
		];

		return new CompositeSpecification<User>(PredicateOperator.OrElse, specifications);
	}

	public static Specification<User> HasRole(string name)
	{
		return new DirectSpecification<User>(user => user.Roles.Any(role => role.Name == name));
	}
}
