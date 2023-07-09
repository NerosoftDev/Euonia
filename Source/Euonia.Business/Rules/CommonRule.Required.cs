namespace Nerosoft.Euonia.Business;

public partial class CommonRule
{
    public class Required : CommonRuleBase
    {
        public Required(IPropertyInfo property, string message)
            : base(property, message)
        { }

        public Required(IPropertyInfo property, Func<string> messageDelegate)
            : base(property, messageDelegate)
        {
        }

        public override async Task ExecuteAsync(IRuleContext context, CancellationToken cancellationToken = default)
        {
            if (context.Target is IBusinessObject target)
            {
                var value = target.ReadProperty(Property);
                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                {
                    var message = string.Format(MessageDelegate(), Property.Name);
                    context.AddErrorResult(message);
                }
            }

            await Task.CompletedTask;
        }
    }
}
