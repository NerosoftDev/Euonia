namespace Nerosoft.Euonia.Business;

public partial class CommonRule
{
    public abstract class CommonRuleBase : RuleBase
    {

        protected CommonRuleBase(IPropertyInfo property)
            : base(property)
        { }

        protected CommonRuleBase(IPropertyInfo property, string message)
            : this(property, () => message)
        {
        }

        protected CommonRuleBase(IPropertyInfo property, Func<string> messageDelegate)
            : this(property)
        {
            MessageDelegate = messageDelegate;
        }

        protected virtual Func<string> MessageDelegate { get; }
    }
}
