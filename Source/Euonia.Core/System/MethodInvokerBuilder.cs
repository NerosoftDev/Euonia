#nullable enable
using System.Linq.Expressions;
using System.Reflection;

namespace System;

/// <summary>
/// Builds a method invoker that can invoke a method dynamically.
/// </summary>
public static class MethodInvokerBuilder
{
	/// <summary>
	/// Builds a method invoker for the specified method.
	/// </summary>
	/// <param name="method"></param>
	/// <returns></returns>
	public static Func<object, object?[], Task<object?>> Build(MethodInfo method)
	{
		var targetExp = Expression.Parameter(typeof(object), "target");
		var argsExp = Expression.Parameter(typeof(object[]), "args");

		var parameters = method.GetParameters();
		var argExps = new Expression[parameters.Length];

		for (var i = 0; i < parameters.Length; i++)
		{
			argExps[i] = Expression.Convert(Expression.ArrayIndex(argsExp, Expression.Constant(i)), parameters[i].ParameterType);
		}

		Expression? instanceExp = method.IsStatic ? null : Expression.Convert(targetExp, method.DeclaringType!);

		var callExp = Expression.Call(instanceExp, method, argExps);

		var body = WrapToTaskObject(callExp, method.ReturnType);

		return Expression.Lambda<Func<object, object?[], Task<object?>>>(body, targetExp, argsExp).Compile();
	}

	/// <summary>
	/// Builds a method invoker for the specified target and method.
	/// </summary>
	/// <param name="target"></param>
	/// <param name="method"></param>
	/// <param name="arguments"></param>
	/// <returns></returns>
	public static Func<Task<object?>> Build(object target, MethodInfo method, params Expression[] arguments)
	{
		var expression = Expression.Call(Expression.Constant(target), method, arguments);

		var body = WrapToTaskObject(expression, method.ReturnType);

		return Expression.Lambda<Func<Task<object?>>>(body).Compile();
	}

	/// <summary>
	/// Builds a call expression for the specified target and method.
	/// </summary>
	/// <param name="target"></param>
	/// <param name="method"></param>
	/// <param name="arguments"></param>
	/// <returns></returns>
	public static Expression BuildCallExpression(object target, MethodInfo method, params Expression[] arguments)
	{
		var expression = Expression.Call(Expression.Constant(target), method, arguments);

		var body = WrapToTaskObject(expression, method.ReturnType);
		return body;
	}

	/// <summary>
	/// Wraps the call expression to return a Task&lt;object&gt;.
	/// </summary>
	/// <param name="callExp"></param>
	/// <param name="returnType"></param>
	/// <returns></returns>
	public static Expression WrapToTaskObject(Expression callExp, Type returnType)
	{
		// ---------- void ----------
		if (returnType == typeof(void))
		{
			return Expression.Block(callExp, Expression.Call(typeof(Task), nameof(Task.FromResult), [typeof(object)], Expression.Constant(Unit.Value, typeof(object))));
		}

		// ---------- Task ----------
		if (returnType == typeof(Task))
		{
			return Expression.Call(typeof(MethodInvokerBuilder), nameof(AwaitTask), null, callExp);
		}

		// ---------- Task<T> ----------
		if (IsGeneric(returnType, typeof(Task<>)))
		{
			var genericReturnType = returnType.GetGenericArguments()[0];
			return Expression.Call(typeof(MethodInvokerBuilder), nameof(AwaitTaskGeneric), [genericReturnType], callExp);
		}

		// ---------- ValueTask ----------
		if (returnType == typeof(ValueTask))
		{
			return Expression.Call(typeof(MethodInvokerBuilder), nameof(AwaitValueTask), null, callExp);
		}

		// ---------- ValueTask<T> ----------
		if (IsGeneric(returnType, typeof(ValueTask<>)))
		{
			var genericReturnType = returnType.GetGenericArguments()[0];
			return Expression.Call(typeof(MethodInvokerBuilder), nameof(AwaitValueTaskGeneric), [genericReturnType], callExp);
		}

		{
		}

		// ---------- Return T ----------
		return Expression.Call(typeof(Task), nameof(Task.FromResult), [typeof(object)], Expression.Convert(callExp, typeof(object)));
	}

	private static bool IsGeneric(Type type, Type openGeneric) => type.IsGenericType && type.GetGenericTypeDefinition() == openGeneric;

	private static async Task<object?> AwaitTask(Task task)
	{
		await task.ConfigureAwait(false);
		return Unit.Value;
	}

	private static async Task<object?> AwaitTaskGeneric<T>(Task<T> task)
	{
		return await task.ConfigureAwait(false);
	}

	private static async Task<object?> AwaitValueTask(ValueTask valueTask)
	{
		await valueTask.ConfigureAwait(false);
		return Unit.Value;
	}

	private static async Task<object?> AwaitValueTaskGeneric<T>(ValueTask<T> valueTask)
	{
		return await valueTask.ConfigureAwait(false);
	}
}