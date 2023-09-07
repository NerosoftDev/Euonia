namespace Nerosoft.Euonia.Business;

public partial class CommonRule
{
    /// <summary>
    /// Provides property required validation.
    /// </summary>
    public class Required : CommonRuleBase
    {
        /// <summary>
        /// Initialize a new instance of <see cref="Required"/>.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="message"></param>
        public Required(IPropertyInfo property, string message)
            : base(property, message)
        { }

        /// <summary>
        /// Initialize a new instance of <see cref="Required"/>.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="messageDelegate"></param>
        public Required(IPropertyInfo property, Func<string> messageDelegate)
            : base(property, messageDelegate)
        {
        }

        /// <inheritdoc />
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
