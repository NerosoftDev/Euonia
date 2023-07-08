using System.Text.RegularExpressions;

namespace Nerosoft.Euonia.Business;

public partial class CommonRule
{
    public class Regular : CommonRuleBase
    {
        private readonly Regex _regex;

        /// <summary>
        /// Initialize a new instance of <see cref="Regular"/>.
        /// </summary>
        /// <param name="property">The property to check.</param>
        /// <param name="expression">The regular expression pattern to match.</param>
        /// <param name="message"></param>
        public Regular(IPropertyInfo property, string expression, string message)
            : base(property, message)
        {
            Expression = expression;
            _regex = new Regex(expression);
        }

        /// <summary>
        /// Initialize a new instance of <see cref="Regular"/>.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="expression">The regular expression pattern to match.</param>
        /// <param name="messageDelegate"></param>
        public Regular(IPropertyInfo property, string expression, Func<string> messageDelegate)
            : base(property, messageDelegate)
        {
            Expression = expression;
            _regex = new Regex(expression);
        }

        /// <summary>
        /// Gets the regular expression pattern to match property value.
        /// </summary>
        public string Expression { get; }

        public bool IgnoreNullValue { get; set; } = true;

        public override async Task ExecuteAsync(IRuleContext context, CancellationToken cancellationToken = default)
        {
            if (context.Target is IBusinessObject target)
            {
                var value = target.ReadProperty(Property);

                var message = value switch
                {
                    string @string => _regex.IsMatch(@string) ? string.Empty : string.Format(MessageDelegate(), Property.Name),
                    null => IgnoreNullValue ? string.Empty : string.Format(MessageDelegate(), Property.Name),
                    _ => throw new NotSupportedException($"The regular expression can not use on property '{Property.Name}'.")
                };
                if (!string.IsNullOrWhiteSpace(message))
                {
                    context.AddErrorResult(message);
                }
            }

            await Task.CompletedTask;
        }
    }
}
