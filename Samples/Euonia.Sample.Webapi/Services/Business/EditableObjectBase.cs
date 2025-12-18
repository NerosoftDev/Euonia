using Nerosoft.Euonia.Business;
using Nerosoft.Euonia.Claims;
using Nerosoft.Euonia.Domain;
using Nerosoft.Euonia.Modularity;

namespace Nerosoft.Euonia.Sample.Business;

/// <summary>
/// Base class for editable business objects that supports lazy service resolution.
/// </summary>
/// <typeparam name="TTarget">The concrete editable object type that derives from this base class.</typeparam>
public abstract class EditableObjectBase<TTarget> : EditableObject<TTarget>, IHasUserPrincipal, IDomainService, IHasLazyServiceProvider
	where TTarget : EditableObjectBase<TTarget>
{
	/// <summary>
	/// Gets or sets the lazy service provider.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <see cref="ILazyServiceProvider"/> allows the object to resolve framework or application
	/// services on demand instead of requiring constructor injection. This property is expected to be
	/// provided by the host or dependency injection infrastructure.
	/// </para>
	/// <para>
	/// This property allows for lazy loading of services, enabling dependency injection and service resolution at runtime.
	/// Please note that this property should be set by the framework or infrastructure code that manages the lifecycle of business objects.
	/// </para>
	/// </remarks>
	//[Inject]
	public ILazyServiceProvider LazyServiceProvider { get; set; }

	/// <summary>
	/// Gets the current user's principal from the lazy service provider.
	/// </summary>
	/// <value>
	/// A <see cref="UserPrincipal"/> representing the authenticated user for the current context.
	/// The principal is resolved from <see cref="LazyServiceProvider"/> each time the property is accessed.
	/// </value>
	public virtual UserPrincipal Identity => LazyServiceProvider.GetRequiredService<UserPrincipal>();

	/// <summary>
	/// Gets the request context accessor from the lazy service provider.
	/// </summary>
	/// <value>
	/// An <see cref="IRequestContextAccessor"/> instance that exposes request-scoped information such as
	/// headers, correlation identifiers, or other ambient request data.
	/// </value>
	protected virtual IRequestContextAccessor RequestContextAccessor => LazyServiceProvider.GetRequiredService<IRequestContextAccessor>();

	/// <summary>
	/// Gets the message bus from the lazy service provider.
	/// </summary>
	/// <value>
	/// An <see cref="IBus"/> used to publish or send domain and integration messages. Resolved on access
	/// through the lazy service provider.
	/// </value>
	protected virtual IBus Bus => LazyServiceProvider.GetRequiredService<IBus>();
}

/// <summary>
/// Base class for editable business objects that are associated with an aggregate root and support lazy service resolution.
/// </summary>
/// <typeparam name="TTarget">The concrete editable object type that derives from this base class.</typeparam>
/// <typeparam name="TAggregate">The aggregate root type associated with this business object.</typeparam>
public abstract class EditableObjectBase<TTarget, TAggregate> : EditableObjectBase<TTarget>
	where TTarget : EditableObjectBase<TTarget, TAggregate>
	where TAggregate : class, IAggregateRoot
{
	/// <summary>
	/// Gets the aggregate root associated with this business object.
	/// </summary>
	protected abstract TAggregate Aggregate { get; }
}
