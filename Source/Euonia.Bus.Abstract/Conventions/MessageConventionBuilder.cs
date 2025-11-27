namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Build customer message convention instead of default message convention.
/// </summary>
public class MessageConventionBuilder
{
	internal MessageConvention Convention { get; } = new();

	/// <summary>
	/// 
	/// </summary>
	/// <param name="convention"></param>
	/// <returns></returns>
	public MessageConventionBuilder EvaluateQueue(Func<Type, bool> convention)
	{
		ArgumentAssert.ThrowIfNull(convention);
		Convention.DefineQueueTypeConvention(convention);
		return this;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="convention"></param>
	/// <returns></returns>
	public MessageConventionBuilder EvaluateTopic(Func<Type, bool> convention)
	{
		ArgumentAssert.ThrowIfNull(convention);
		Convention.DefineTopicTypeConvention(convention);
		return this;
	}

	/// <summary>
	/// Evaluate request type convention.
	/// </summary>
	/// <param name="convention"></param>
	/// <returns></returns>
	public MessageConventionBuilder EvaluateRequest(Func<Type, bool> convention)
	{
		ArgumentAssert.ThrowIfNull(convention);
		Convention.DefineRequestTypeConvention(convention);
		return this;
	}

	/// <summary>
	/// Adds a message convention that will be used to evaluate whether a type is a message, command, or event.
	/// </summary>
	/// <param name="convention">The message convention instance.</param>
	/// <typeparam name="TConvention">The message convention type.</typeparam>
	/// <returns></returns>
	public MessageConventionBuilder Add<TConvention>(TConvention convention)
		where TConvention : class, IMessageConvention
	{
		ArgumentAssert.ThrowIfNull(convention);
		Convention.Add(convention);
		return this;
	}

	/// <summary>
	/// Adds a message convention that will be used to evaluate whether a type is a message, command, or event.
	/// </summary>
	/// <typeparam name="TConvention">The message convention type.</typeparam>
	/// <returns></returns>
	public MessageConventionBuilder Add<TConvention>()
		where TConvention : class, IMessageConvention, new()
	{
		Convention.Add(new TConvention());
		return this;
	}
}