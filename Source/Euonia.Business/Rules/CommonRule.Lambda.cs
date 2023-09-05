namespace Nerosoft.Euonia.Business;

/// <summary>
/// Provides common rule set to validate a property.
/// </summary>
public partial class CommonRule
{
    /// <summary>
    /// Provides property validation using a lambda expression.
    /// </summary>
    public class Lambda : CommonRuleBase
    {
	    /// <inheritdoc />
	    public Lambda(IPropertyInfo property, Func<object, IRuleContext, bool> handler, string message)
            : base(property, message)
        {
            Handler = handler;
        }

	    /// <inheritdoc />
	    public Lambda(IPropertyInfo property, Func<object, IRuleContext, bool> handler, Func<string> messageDelegate)
            : base(property, messageDelegate)
        {
            Handler = handler;
        }

        private Func<object, IRuleContext, bool> Handler { get; }

        /// <inheritdoc />
        public override async Task ExecuteAsync(IRuleContext context, CancellationToken cancellationToken = default)
        {
            if (context.Target is IBusinessObject target)
            {
                var value = target.ReadProperty(Property);

                var result = Handler(value, context);

                if (!result)
                {
                    context.AddErrorResult(string.Format(MessageDelegate(), Property.Name));
                }
            }

            await Task.CompletedTask;
        }
    }

    /// <summary>
    /// Provides property validation using a lambda expression.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Lambda<T> : CommonRuleBase
    {
	    /// <inheritdoc />
	    public Lambda(PropertyInfo<T> property, Func<T, IRuleContext, bool> handler, string message)
            : base(property, message)
        {
            Handler = handler;
        }

	    /// <inheritdoc />
	    public Lambda(PropertyInfo<T> property, Func<T, IRuleContext, bool> handler, Func<string> messageDelegate)
            : base(property, messageDelegate)
        {
            Handler = handler;
        }

        private Func<T, IRuleContext, bool> Handler { get; }

        /// <inheritdoc />
        public override async Task ExecuteAsync(IRuleContext context, CancellationToken cancellationToken = default)
        {
            if (context.Target is IBusinessObject target)
            {
                var value = (T)target.ReadProperty(Property);

                var result = Handler(value, context);

                if (!result)
                {
                    context.AddErrorResult(string.Format(MessageDelegate(), Property.Name));
                }
            }

            await Task.CompletedTask;
        }
    }
}