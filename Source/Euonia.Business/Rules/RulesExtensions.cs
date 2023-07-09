namespace Nerosoft.Euonia.Business;

public static class RulesExtensions
{
    /// <summary>
    /// Add a lambda expression rule to business object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="rules"></param>
    /// <param name="property"></param>
    /// <param name="handler"></param>
    /// <param name="message"></param>
    public static void AddRule<T>(this Rules rules, IPropertyInfo property, Func<T, bool> handler, string message)
        where T : BusinessObject
    {
        var rule = new CommonRule.Lambda(property, (_, context) =>
        {
            var target = (T)context.Target;
            using (target.BypassRuleChecks)
            {
                return handler(target);
            }
        }, message);
        //var methodName = handler.Method.ToString();
        //rule.AddQueryParameter("s", Convert.ToBase64String(Encoding.Unicode.GetBytes(methodName)));

        rules.AddRule(rule);
    }

    /// <summary>
    /// Add a lambda expression rule to business object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="rules"></param>
    /// <param name="property"></param>
    /// <param name="handler"></param>
    /// <param name="message"></param>
    public static void AddRule<T>(this Rules rules, IPropertyInfo property, Func<T, bool> handler, Func<string> message)
        where T : BusinessObject
    {
        var rule = new CommonRule.Lambda(property, (_, context) =>
        {
            var target = (T)context.Target;
            using (target.BypassRuleChecks)
            {
                return handler(target);
            }
        }, message);
        //var methodName = handler.Method.ToString();
        //rule.AddQueryParameter("s", Convert.ToBase64String(Encoding.Unicode.GetBytes(methodName)));

        rules.AddRule(rule);
    }
}
