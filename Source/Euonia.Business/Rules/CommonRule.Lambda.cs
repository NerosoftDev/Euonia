namespace Nerosoft.Euonia.Business;

public partial class CommonRule
{
    public class Lambda : CommonRuleBase
    {
        public Lambda(IPropertyInfo property, Func<object, IRuleContext, bool> handler, string message)
            : base(property, message)
        {
            Handler = handler;
        }

        public Lambda(IPropertyInfo property, Func<object, IRuleContext, bool> handler, Func<string> messageDelegate)
            : base(property, messageDelegate)
        {
            Handler = handler;
        }

        protected Func<object, IRuleContext, bool> Handler { get; }

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

    public class Lambda<T> : CommonRuleBase
    {
        public Lambda(PropertyInfo<T> property, Func<T, IRuleContext, bool> handler, string message)
            : base(property, message)
        {
            Handler = handler;
        }

        public Lambda(PropertyInfo<T> property, Func<T, IRuleContext, bool> handler, Func<string> messageDelegate)
            : base(property, messageDelegate)
        {
            Handler = handler;
        }

        protected Func<T, IRuleContext, bool> Handler { get; }

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