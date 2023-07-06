using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace Nerosoft.Euonia.Linq;

/// <summary>
/// Extension methods for <see cref="Expression"/>.
/// </summary>
public static class ExpressionExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="expressions"></param>
    /// <param name="condition"></param>
    /// <param name="expression"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IList<Expression<Func<T, bool>>> AddIf<T>(this IList<Expression<Func<T, bool>>> expressions, bool condition, Expression<Func<T, bool>> expression)
    {
        if (condition)
        {
            expressions.Add(expression);
        }

        return expressions;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="expressions"></param>
    /// <param name="condition"></param>
    /// <param name="expression"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IList<Expression<Func<T, bool>>> AddIf<T>(this IList<Expression<Func<T, bool>>> expressions, Func<bool> condition, Expression<Func<T, bool>> expression)
    {
        return expressions.AddIf(condition(), expression);
    }

    /// <summary>
    /// Combine all expressions into a new expression.
    /// </summary>
    /// <param name="expressions"></param>
    /// <param name="type"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Expression<Func<T, bool>> Compose<T>(this IEnumerable<Expression<Func<T, bool>>> expressions, PredicateOperator type = PredicateOperator.AndAlso)
    {
        return expressions.Compose(PredicateBuilder.True<T>(), type);
    }

    /// <summary>
    /// Combine all expressions into a new expression.
    /// </summary>
    /// <param name="expressions"></param>
    /// <param name="seed"></param>
    /// <param name="type"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Expression<Func<T, bool>> Compose<T>(this IEnumerable<Expression<Func<T, bool>>> expressions, Expression<Func<T, bool>> seed, PredicateOperator type = PredicateOperator.AndAlso)
    {
        var predicate = expressions.Aggregate(seed, (current, next) => Compose(current, next, type));
        return predicate;
    }

    private static Expression<Func<T, bool>> Compose<T>(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right, PredicateOperator type)
    {
        return type switch
        {
            PredicateOperator.AndAlso => left.And(right),
            PredicateOperator.OrElse => left.Or(right),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    // private static Expression<Func<T,bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    // {
    //     var parameter = Expression.Parameter(typeof(T));
    //
    //     var leftVisitor = new ReplaceExpressionVisitor(left.Parameters[0], parameter);
    //     var leftExpression = leftVisitor.Visit(left.Body);
    //
    //     var rightVisitor = new ReplaceExpressionVisitor(right.Parameters[0], parameter);
    //     var rightExpression = rightVisitor.Visit(right.Body);
    //
    //     return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(leftExpression, rightExpression), parameter);
    // }

    #region Property(属性表达式)

    /// <summary>
    /// Creates a <see cref="MemberExpression"/> for the specified property.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="propertyName">The property name. e.g: Name, Customer.Name</param>
    public static Expression Property(this Expression expression, string propertyName)
    {
        if (propertyName.All(t => t != '.'))
            return Expression.Property(expression, propertyName);
        var propertyNameList = propertyName.Split('.');
        Expression result = null;
        for (var i = 0; i < propertyNameList.Length; i++)
        {
            if (i == 0)
            {
                result = Expression.Property(expression, propertyNameList[0]);
                continue;
            }

            result = result.Property(propertyNameList[i]);
        }

        return result;
    }

    /// <summary>
    /// Create a <see cref="MemberExpression"/> for the specified property.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="member">The property member.</param>
    public static Expression Property(this Expression expression, MemberInfo member)
    {
        return Expression.MakeMemberAccess(expression, member);
    }

    #endregion

    #region And expression
    /// <summary>
    /// Combines the first predicate with the second using the logical "and".
    /// </summary>
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
    {
        return first.Compose(second, Expression.AndAlso);
    }

    ///*
    /// <summary>
    /// 与操作表达式
    /// </summary>
    /// <param name="left">左操作数</param>
    /// <param name="right">右操作数</param>
    public static Expression And(this Expression left, Expression right)
    {
        if (left == null)
            return right;
        if (right == null)
            return left;
        return Expression.AndAlso(left, right);
    }

    #endregion

    #region Or expression

    /// <summary>
    /// Combines the first predicate with the second using the logical "or".
    /// </summary>
    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
    {
        if (first == null)
        {
            return second;
        }
        if (second == null)
        {
            return first;
        }
        return first.Compose(second, Expression.OrElse);
    }

    /// <summary>
    /// 或操作表达式
    /// </summary>
    /// <param name="first">左操作数</param>
    /// <param name="second">右操作数</param>
    public static Expression Or(this Expression first, Expression second)
    {
        return Expression.OrElse(first, second);
    }

    //*/
    #endregion

    #region Value

    /// <summary>
    /// Gets value from a lambda expression.
    /// </summary>
    /// <typeparam name="T">The target object type.</typeparam>
    public static object Value<T>(this Expression<Func<T, bool>> expression)
    {
        return Lambda.GetValue(expression);
    }

    #endregion

    #region Equal

    /// <summary>
    /// To be added.
    /// </summary>
    /// <param name="left">To be added.</param>
    /// <param name="right">To be added.</param>
    public static Expression Equal(this Expression left, Expression right)
    {
        return Expression.Equal(left, right);
    }

    /// <summary>
    /// To be added.
    /// </summary>
    /// <param name="left">To be added.</param>
    /// <param name="value">To be added.</param>
    public static Expression Equal(this Expression left, object value)
    {
        return left.Equal(Lambda.Constant(left, value));
    }

    #endregion

    #region NotEqual

    /// <summary>
    /// To be added.
    /// </summary>
    /// <param name="left">To be added.</param>
    /// <param name="right">To be added.</param>
    public static Expression NotEqual(this Expression left, Expression right)
    {
        return Expression.NotEqual(left, right);
    }

    /// <summary>
    /// To be added.
    /// </summary>
    /// <param name="left">To be added.</param>
    /// <param name="value">To be added.</param>
    public static Expression NotEqual(this Expression left, object value)
    {
        return left.NotEqual(Lambda.Constant(left, value));
    }

    #endregion

    #region Greater

    /// <summary>
    /// To be added.
    /// </summary>
    /// <param name="left">To be added.</param>
    /// <param name="right">To be added.</param>
    public static Expression Greater(this Expression left, Expression right)
    {
        return Expression.GreaterThan(left, right);
    }

    /// <summary>
    /// To be added.
    /// </summary>
    /// <param name="left">To be added.</param>
    /// <param name="value">To be added.</param>
    public static Expression Greater(this Expression left, object value)
    {
        return left.Greater(Lambda.Constant(left, value));
    }

    #endregion

    #region GreaterEqual

    /// <summary>
    /// To be added.
    /// </summary>
    /// <param name="left">To be added.</param>
    /// <param name="right">To be added.</param>
    public static Expression GreaterEqual(this Expression left, Expression right)
    {
        return Expression.GreaterThanOrEqual(left, right);
    }

    /// <summary>
    /// To be added.
    /// </summary>
    /// <param name="left">To be added.</param>
    /// <param name="value">To be added.</param>
    public static Expression GreaterEqual(this Expression left, object value)
    {
        return left.GreaterEqual(Lambda.Constant(left, value));
    }

    #endregion

    #region Less

    /// <summary>
    /// To be added.
    /// </summary>
    /// <param name="left">To be added.</param>
    /// <param name="right">To be added.</param>
    public static Expression Less(this Expression left, Expression right)
    {
        return Expression.LessThan(left, right);
    }

    /// <summary>
    /// To be added.
    /// </summary>
    /// <param name="left">To be added.</param>
    /// <param name="value">To be added.</param>
    public static Expression Less(this Expression left, object value)
    {
        return left.Less(Lambda.Constant(left, value));
    }

    #endregion

    #region LessEqual

    /// <summary>
    /// To be added.
    /// </summary>
    /// <param name="left">To be added.</param>
    /// <param name="right">To be added.</param>
    public static Expression LessEqual(this Expression left, Expression right)
    {
        return Expression.LessThanOrEqual(left, right);
    }

    /// <summary>
    /// To be added.
    /// </summary>
    /// <param name="left">To be added.</param>
    /// <param name="value">To be added.</param>
    public static Expression LessEqual(this Expression left, object value)
    {
        return left.LessEqual(Lambda.Constant(left, value));
    }

    #endregion

    #region StartsWith

    /// <summary>
    /// To be added.
    /// </summary>
    /// <param name="left">To be added.</param>
    /// <param name="value">To be added.</param>
    public static Expression StartsWith(this Expression left, object value)
    {
        return left.Call("StartsWith", new[] { typeof(string) }, value);
    }

    #endregion

    #region EndsWith(尾匹配)

    /// <summary>
    /// 尾匹配
    /// </summary>
    /// <param name="left">左操作数</param>
    /// <param name="value">值</param>
    public static Expression EndsWith(this Expression left, object value)
    {
        return left.Call("EndsWith", new[] { typeof(string) }, value);
    }

    #endregion

    #region Contains(模糊匹配)

    /// <summary>
    /// 模糊匹配
    /// </summary>
    /// <param name="left">左操作数</param>
    /// <param name="value">值</param>
    public static Expression Contains(this Expression left, object value)
    {
        return left.Call("Contains", new[] { typeof(string) }, value);
    }

    #endregion

    #region Operation(操作)

    /// <summary>
    /// To be added.
    /// </summary>
    /// <param name="left">To be added.</param>
    /// <param name="operator">To be added.</param>
    /// <param name="value">To be added.</param>
    public static Expression Operation(this Expression left, QueryOperator @operator, object value)
    {
        return @operator switch
        {
            QueryOperator.Equal => left.Equal(value),
            QueryOperator.NotEqual => left.NotEqual(value),
            QueryOperator.GreaterThan => left.Greater(value),
            QueryOperator.GreaterThanOrEqual => left.GreaterEqual(value),
            QueryOperator.LessThan => left.Less(value),
            QueryOperator.LessThanOrEqual => left.LessEqual(value),
            QueryOperator.StartsWith => left.StartsWith(value),
            QueryOperator.EndsWith => left.EndsWith(value),
            QueryOperator.Contains => left.Contains(value),
            QueryOperator.NotContains => throw new NotImplementedException(),
            QueryOperator.Like => throw new NotImplementedException(),
            QueryOperator.NotLike => throw new NotImplementedException(),
            QueryOperator.Is => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        };
    }

    #endregion

    #region Call(调用方法表达式)

    /// <summary>
    /// To be added.
    /// </summary>
    /// <param name="instance">To be added.</param>
    /// <param name="methodName">To be added.</param>
    /// <param name="values">To be added.</param>
    public static Expression Call(this Expression instance, string methodName, params Expression[] values)
    {
        if (instance == null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        var method = instance.Type.GetTypeInfo().GetMethod(methodName);

        if (method == null)
        {
            throw new NullReferenceException($"Method {methodName} not found.");
        }

        return Expression.Call(instance, method, values);
    }

    /// <summary>
    /// To be added.
    /// </summary>
    /// <param name="instance">To be added.</param>
    /// <param name="methodName">To be added.</param>
    /// <param name="values">To be added.</param>
    public static Expression Call(this Expression instance, string methodName, params object[] values)
    {
        if (instance == null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        var method = instance.Type.GetTypeInfo().GetMethod(methodName);

        if (method == null)
        {
            throw new NullReferenceException($"Method {methodName} not found.");
        }

        if (values == null || values.Length == 0)
        {
            return Expression.Call(instance, method);
        }

        return Expression.Call(instance, method, values.Select(Expression.Constant));
    }

    /// <summary>
    /// To be added.
    /// </summary>
    /// <param name="instance">To be added.</param>
    /// <param name="methodName">To be added.</param>
    /// <param name="paramTypes">To be added.</param>
    /// <param name="values">To be added.</param>
    public static Expression Call(this Expression instance, string methodName, Type[] paramTypes, params object[] values)
    {
        if (instance == null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        var method = instance.Type.GetTypeInfo().GetMethod(methodName, paramTypes);

        if (method == null)
        {
            throw new NullReferenceException($"Method {methodName} not found.");
        }

        if (values == null || values.Length == 0)
        {
            return Expression.Call(instance, method);
        }

        return Expression.Call(instance, method, values.Select(Expression.Constant));
    }

    #endregion

    #region Compose(组合表达式)

    /// <summary>
    /// Combines the first expression with the second using the specified merge function.
    /// </summary>
    private static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
    {
        // zip parameters (map from parameters of second to parameters of first)
        var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] })
                                  .ToDictionary(p => p.s, p => p.f);

        // replace parameters in the second lambda expression with the parameters in the first
        var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

        // create a merged lambda expression with parameters from the first expression
        return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
    }

    #endregion

    #region ToLambda(创建Lambda表达式)

    /// <summary>
    /// Creates a lambda expression from the specified expression and parameters.
    /// </summary>
    /// <typeparam name="TDelegate">委托类型</typeparam>
    /// <param name="body">表达式</param>
    /// <param name="parameters">参数列表</param>
    public static Expression<TDelegate> ToLambda<TDelegate>(this Expression body, params ParameterExpression[] parameters)
    {
        if (body == null)
        {
            return null;
        }
        return Expression.Lambda<TDelegate>(body, parameters);
    }

    #endregion

    /// <summary>
    /// Negates the predicate.
    /// </summary>
    public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
    {
        var negated = Expression.Not(expression.Body);
        return Expression.Lambda<Func<T, bool>>(negated, expression.Parameters);
    }

    /// <summary>
    /// Extends the specified source Predicate with another Predicate and the specified PredicateOperator.
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    /// <param name="first">The source Predicate.</param>
    /// <param name="second">The second Predicate.</param>
    /// <param name="operator">The Operator (can be "And" or "Or").</param>
    /// <returns>Expression{Func{T, bool}}</returns>
    public static Expression<Func<T, bool>> Extend<T>([NotNull] this Expression<Func<T, bool>> first, [NotNull] Expression<Func<T, bool>> second, PredicateOperator @operator = PredicateOperator.AndAlso)
    {
        return @operator == PredicateOperator.OrElse ? first.Or(second) : first.And(second);
    }
}
