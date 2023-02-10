using System.Linq.Expressions;
using System.Reflection;

namespace Nerosoft.Euonia.Reflection;

/// <summary>
/// Provides strong-typed reflection
/// </summary>
public static class Reflect
{
    /// <summary>
    /// Extracts the property from a property expression.
    /// </summary>
    /// <typeparam name="T">The object type containing the property specified in the expression.</typeparam>
    /// <param name="expression">The property expression (e.g. p =&gt; p.PropertyName)</param>
    /// <returns>The name of the property.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="expression" /> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the expression is:<br />
    /// Not a <see cref="MemberExpression" /><br />
    /// The <see cref="MemberExpression" /> does not represent a property.<br />
    /// Or, the property is static.</exception>
    public static PropertyInfo GetProperty<T>(Expression<Func<T>> expression)
    {
        if (expression == null)
        {
            throw new ArgumentNullException(nameof(expression));
        }

        if (expression.Body is not MemberExpression memberExpression)
        {
            throw new ArgumentException("The expression is not a member access expression.", nameof(expression));
        }

        var property = memberExpression.Member as PropertyInfo;
        if (property == null)
        {
            throw new ArgumentException("The member access expression does not access a property.", nameof(expression));
        }

        return property;
    }

    /// <summary>
    /// Extracts the property from a property expression.
    /// </summary>
    /// <param name="expression"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static PropertyInfo GetProperty<T>(Expression<Func<T, object>> expression)
    {
        return GetProperty<T, object>(expression);
    }

    /// <summary>
    /// Extracts the property from a property expression.
    /// </summary>
    /// <param name="expression"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static PropertyInfo GetProperty<T, TResult>(Expression<Func<T, TResult>> expression)
    {
        if (expression == null)
        {
            throw new ArgumentNullException(nameof(expression));
        }

        PropertyInfo result;

        if (expression.Body.NodeType == ExpressionType.Convert)
        {
            result = ((MemberExpression)((UnaryExpression)expression.Body).Operand).Member as PropertyInfo;
        }
        else
        {
            result = ((MemberExpression)expression.Body).Member as PropertyInfo;
        }

        if (result != null)
        {
            return result;
        }

        throw new ArgumentException($"Expression '{expression}' does not refer to a property.");
    }

    public static MethodInfo GetMethodInfo<T>(Expression<Func<T, Delegate>> expression)
    {
        return GetMethodInfo((LambdaExpression)expression);
    }

    //public static MethodInfo GetMethodInfo(LambdaExpression expression)
    //{
    //    var unaryExpression = (UnaryExpression)expression.Body;
    //    var methodCallExpression = (MethodCallExpression)unaryExpression.Operand;
    //    var methodCallObject = (ConstantExpression)methodCallExpression.Object;
    //    var methodInfo = (MethodInfo)methodCallObject.Value;
    //    return methodInfo;
    //}

    public static MethodInfo GetMethodInfo(Expression expression)
    {
        if (expression == null)
        {
            throw new ArgumentNullException(nameof(expression));
        }
        if (expression is not LambdaExpression lambda)
        {
            throw new ArgumentException("Not a lambda expression", nameof(expression));
        }

        if (lambda.Body.NodeType == ExpressionType.Convert)
        {
            var unaryExpression = (UnaryExpression)lambda.Body;
            var methodCallExpression = (MethodCallExpression)unaryExpression.Operand;
            var methodCallObject = (ConstantExpression)methodCallExpression.Object;
            if (methodCallObject == null)
            {
                throw new NullReferenceException(nameof(methodCallObject));
            }
            var methodInfo = (MethodInfo)methodCallObject.Value;
            return methodInfo;
        }
        else if (lambda.Body.NodeType == ExpressionType.Call)
        {
            return ((MethodCallExpression)lambda.Body).Method;
        }

        throw new ArgumentException("Not a method call", nameof(expression));
    }

    /// <summary>
    /// Get the member information represented by the lambda expression.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static MemberInfo GetMemberInfo(Expression expression)
    {
        if (expression == null)
        {
            throw new ArgumentNullException(nameof(expression));
        }

        if (expression is not LambdaExpression lambda)
        {
            throw new ArgumentException("Not a lambda expression", nameof(expression));
        }

        MemberExpression memberExpr = null;

        // The Func<TTarget, object> we use returns an object, so first statement can be either 
        // a cast (if the field/property does not return an object) or the direct member access.
        if (lambda.Body.NodeType == ExpressionType.Convert)
        {
            // The cast is an unary expression, where the operand is the 
            // actual member access expression.
            memberExpr = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
        }
        else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
        {
            memberExpr = lambda.Body as MemberExpression;
        }

        if (memberExpr == null)
            throw new ArgumentException("Not a member access", nameof(expression));

        return memberExpr.Member;
    }
}

/// <summary>
/// Provides strong-typed reflection of the <typeparamref name="TTarget"/> 
/// type.
/// </summary>
/// <typeparam name="TTarget">Type to reflect.</typeparam>
public static class Reflect<TTarget>
{
    /// <summary>
    /// Gets the method represented by the lambda expression.
    /// </summary>
    /// <exception cref="ArgumentNullException">The <paramref name="expression"/> is null.</exception>
    /// <exception cref="ArgumentException">The <paramref name="expression"/> is not a lambda expression or it does not represent a method invocation.</exception>
    public static MethodInfo GetMethod(Expression<Action<TTarget>> expression)
    {
        return Reflect.GetMethodInfo(expression);
    }

    /// <summary>
    /// Gets the method represented by the lambda expression.
    /// </summary>
    /// <exception cref="ArgumentNullException">The <paramref name="expression"/> is null.</exception>
    /// <exception cref="ArgumentException">The <paramref name="expression"/> is not a lambda expression or it does not represent a method invocation.</exception>
    public static MethodInfo GetMethod<T1>(Expression<Action<TTarget, T1>> expression)
    {
        return Reflect.GetMethodInfo(expression);
    }

    /// <summary>
    /// Gets the method represented by the lambda expression.
    /// </summary>
    /// <exception cref="ArgumentNullException">The <paramref name="expression"/> is null.</exception>
    /// <exception cref="ArgumentException">The <paramref name="expression"/> is not a lambda expression or it does not represent a method invocation.</exception>
    public static MethodInfo GetMethod<T1, T2>(Expression<Action<TTarget, T1, T2>> expression)
    {
        return Reflect.GetMethodInfo(expression);
    }

    /// <summary>
    /// Gets the method represented by the lambda expression.
    /// </summary>
    /// <exception cref="ArgumentNullException">The <paramref name="expression"/> is null.</exception>
    /// <exception cref="ArgumentException">The <paramref name="expression"/> is not a lambda expression or it does not represent a method invocation.</exception>
    public static MethodInfo GetMethod<T1, T2, T3>(Expression<Action<TTarget, T1, T2, T3>> expression)
    {
        return Reflect.GetMethodInfo(expression);
    }

    /// <summary>
    /// Gets the property represented by the lambda expression.
    /// </summary>
    /// <exception cref="ArgumentNullException">The <paramref name="expression"/> is null.</exception>
    /// <exception cref="ArgumentException">The <paramref name="expression"/> is not a lambda expression or it does not represent a property access.</exception>
    public static PropertyInfo GetProperty(Expression<Func<TTarget, object>> expression)
    {
        var info = Reflect.GetMemberInfo(expression) as PropertyInfo;
        if (info == null)
            throw new ArgumentException("Member is not a property");

        return info;
    }

    /// <summary>
    /// Gets the property represented by the lambda expression.
    /// </summary>
    /// <typeparam name="TValue">Type assigned to the property</typeparam>
    /// <param name="expression">Property Expression</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">The <paramref name="expression"/> is null.</exception>
    /// <exception cref="ArgumentException">The <paramref name="expression"/> is not a lambda expression or it does not represent a property access.</exception>
    public static PropertyInfo GetProperty<TValue>(Expression<Func<TTarget, TValue>> expression)
    {
        var info = Reflect.GetMemberInfo(expression) as PropertyInfo;
        if (info == null)
            throw new ArgumentException("Member is not a property");

        return info;
    }

    /// <summary>
    /// Gets the field represented by the lambda expression.
    /// </summary>
    /// <exception cref="ArgumentNullException">The <paramref name="expression"/> is null.</exception>
    /// <exception cref="ArgumentException">The <paramref name="expression"/> is not a lambda expression or it does not represent a field access.</exception>
    public static FieldInfo GetField(Expression<Func<TTarget, object>> expression)
    {
        var info = Reflect.GetMemberInfo(expression) as FieldInfo;
        if (info == null)
            throw new ArgumentException("Member is not a field");

        return info;
    }
}
