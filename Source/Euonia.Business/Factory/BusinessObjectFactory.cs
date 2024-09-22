﻿using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Nerosoft.Euonia.Business;

/// <summary>
/// The business object factory.
/// </summary>
public class BusinessObjectFactory : IObjectFactory
{
	private readonly IServiceProvider _provider;
	private readonly IObjectActivator _activator;

	/// <summary>
	/// Initialize a new instance of <see cref="BusinessObjectFactory"/>.
	/// </summary>
	/// <param name="provider"></param>
	public BusinessObjectFactory(IServiceProvider provider)
	{
		_provider = provider;
	}

	/// <summary>
	/// Initialize a new instance of <see cref="BusinessObjectFactory"/>.
	/// </summary>
	/// <param name="provider"></param>
	/// <param name="activator"></param>
	public BusinessObjectFactory(IServiceProvider provider, IObjectActivator activator)
	{
		_provider = provider;
		_activator = activator;
	}

	/// <inheritdoc/>
	public async Task<TTarget> CreateAsync<TTarget>(params object[] criteria)
	{
		criteria ??= [null];
		var method = ObjectReflector.FindFactoryMethod<TTarget, FactoryCreateAttribute>(criteria);
		var target = GetObjectInstance<TTarget>();
		if (target is IEditableObject editable)
		{
			editable.MarkAsInsert();
		}

		try
		{
			_activator?.InitializeInstance(target);
			await InvokeAsync(method, target, criteria);
			return target;
		}
		finally
		{
			_activator?.FinalizeInstance(target);
		}
	}

	/// <inheritdoc/>
	public async Task<TTarget> FetchAsync<TTarget>(params object[] criteria)
	{
		criteria ??= [null];
		var method = ObjectReflector.FindFactoryMethod<TTarget, FactoryFetchAttribute>(criteria);
		var target = GetObjectInstance<TTarget>();
		try
		{
			_activator?.InitializeInstance(target);
			await InvokeAsync(method, target, criteria);
			return target;
		}
		finally
		{
			_activator?.FinalizeInstance(target);
		}
	}

	/// <inheritdoc/>
	public async Task<TTarget> InsertAsync<TTarget>(params object[] criteria)
	{
		criteria ??= [null];
		var method = ObjectReflector.FindFactoryMethod<TTarget, FactoryInsertAttribute>(criteria);
		var target = GetObjectInstance<TTarget>();
		try
		{
			_activator?.InitializeInstance(target);
			await InvokeAsync(method, target, criteria);
			return target;
		}
		finally
		{
			_activator?.FinalizeInstance(target);
		}
	}

	/// <inheritdoc/>
	public async Task<TTarget> UpdateAsync<TTarget>(params object[] criteria)
	{
		criteria ??= [null];
		var method = ObjectReflector.FindFactoryMethod<TTarget, FactoryUpdateAttribute>(criteria);
		var target = GetObjectInstance<TTarget>();
		try
		{
			_activator?.InitializeInstance(target);
			await InvokeAsync(method, target, criteria);
			return target;
		}
		finally
		{
			_activator?.FinalizeInstance(target);
		}
	}

	/// <inheritdoc/>
	public async Task<TTarget> SaveAsync<TTarget>(TTarget target, CancellationToken cancellationToken = default)
	{
		var method = target switch
		{
			IEditableObject editableObject => editableObject.State switch
			{
				ObjectEditState.Insert => ObjectReflector.FindFactoryMethod<TTarget, FactoryInsertAttribute>([cancellationToken]),
				ObjectEditState.Update => ObjectReflector.FindFactoryMethod<TTarget, FactoryUpdateAttribute>([cancellationToken]),
				ObjectEditState.Delete => ObjectReflector.FindFactoryMethod<TTarget, FactoryDeleteAttribute>([cancellationToken]),
				ObjectEditState.None => throw new InvalidOperationException(),
				_ => throw new ArgumentOutOfRangeException(nameof(target), Resources.IDS_INVALID_STATE)
			},
			ICommandObject => ObjectReflector.FindFactoryMethod<TTarget, FactoryExecuteAttribute>([cancellationToken]),
			IReadOnlyObject => throw new InvalidOperationException("The operation can not apply for ReadOnlyObject."),
			_ => ObjectReflector.FindFactoryMethod<TTarget, FactoryUpdateAttribute>([cancellationToken])
		};

		await InvokeAsync(method, target, [cancellationToken]);

		return target;
	}

	/// <inheritdoc/>
	public async Task<TTarget> ExecuteAsync<TTarget>(TTarget target, CancellationToken cancellationToken = default)
		where TTarget : ICommandObject
	{
		var method = ObjectReflector.FindFactoryMethod<TTarget, FactoryExecuteAttribute>([cancellationToken]);

		try
		{
			_activator?.InitializeInstance(target);
			await InvokeAsync(method, target, [cancellationToken]);
			return target;
		}
		finally
		{
			_activator?.FinalizeInstance(target);
		}
	}

	/// <inheritdoc/>
	public async Task<TTarget> ExecuteAsync<TTarget>(params object[] criteria)
		where TTarget : ICommandObject
	{
		criteria ??= [null];
		var method = ObjectReflector.FindFactoryMethod<TTarget, FactoryExecuteAttribute>(criteria);
		var target = GetObjectInstance<TTarget>();

		try
		{
			_activator?.InitializeInstance(target);
			await InvokeAsync(method, target, criteria);
			return target;
		}
		finally
		{
			_activator?.FinalizeInstance(target);
		}
	}

	/// <inheritdoc/>
	public async Task DeleteAsync<TTarget>(params object[] criteria)
	{
		criteria ??= [null];
		var method = ObjectReflector.FindFactoryMethod<TTarget, FactoryDeleteAttribute>(criteria);
		var target = GetObjectInstance<TTarget>();

		try
		{
			_activator?.InitializeInstance(target);
			await InvokeAsync(method, target, criteria);
		}
		finally
		{
			_activator?.FinalizeInstance(target);
		}
	}

	#region Supports

	private static async Task InvokeAsync<TTarget>(MethodInfo method, TTarget target, object[] parameters)
	{
		if (method.IsAsync())
		{
			await ((Task)method.Invoke(target, parameters: parameters))!;
		}
		else
		{
			method.Invoke(target, parameters: parameters);
		}
	}

	/// <summary>
	/// Get instance from <see cref="IServiceProvider"/> or create a new instance .
	/// </summary>
	/// <typeparam name="TTarget"></typeparam>
	/// <returns></returns>
	private TTarget GetObjectInstance<TTarget>()
	{
		var @object = ActivatorUtilities.GetServiceOrCreateInstance<TTarget>(_provider);
		switch (@object)
		{
			case IUseBusinessContext ctx:
				ctx.BusinessContext = _provider.GetRequiredService<BusinessContext>();
				break;
			case IHasLazyServiceProvider lazy:
				lazy.LazyServiceProvider = _provider.GetRequiredService<ILazyServiceProvider>();
				break;
		}

		var properties = ObjectReflector.GetAutoInjectProperties(typeof(TTarget));

		foreach (var (property, type, multiple, serviceKey) in properties)
		{
			if (multiple)
			{
				var implement = serviceKey == null ? _provider.GetServices(type) : _provider.GetKeyedServices(type, serviceKey);
				property.SetValue(@object, implement);
			}
			else
			{
				var implement = serviceKey == null ? _provider.GetService(type) : _provider.GetKeyedService(type, serviceKey);
				property.SetValue(@object, implement);
			}
		}

		return @object;
	}

	#endregion
}