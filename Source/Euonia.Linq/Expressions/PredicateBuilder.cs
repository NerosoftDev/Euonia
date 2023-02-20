﻿using System.Linq.Expressions;
using System.Reflection;

namespace Nerosoft.Euonia.Linq;

/// <summary>
/// Enables the efficient, dynamic composition of query predicates.
/// </summary>
/// <remarks>
/// See http://petemontgomery.wordpress.com/2011/02/10/a-universal-predicatebuilder/
/// </remarks>
public static class PredicateBuilder
{
    /// <summary>
    /// Creates a predicate that evaluates to true.
    /// </summary>
    public static Expression<Func<T, bool>> True<T>() { return param => true; }

    /// <summary>
    /// Creates a predicate that evaluates to false.
    /// </summary>
    public static Expression<Func<T, bool>> False<T>() { return param => false; }

    /// <summary>
    /// Creates a predicate expression from the specified lambda expression.
    /// </summary>
    public static Expression<Func<T, bool>> Create<T>(Expression<Func<T, bool>> predicate) { return predicate; }

    /// <summary>
    /// Gets the compare condition.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TValue">The type of the t value.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="value">The value.</param>
    /// <param name="operator">Type of the compare.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static Expression<Func<T, bool>> GetCompareCondition<T, TValue>(T source, string propertyName, TValue value, QueryOperator @operator)
    {
        var param = Expression.Parameter(typeof(T), "p");
        var exp = Expression.Constant(value);
        var structure = propertyName.Split('.').ToList();
        MemberExpression member = SearchMember(param, structure);
        Expression condition;

        switch (@operator)
        {
            case QueryOperator.Equal:
                condition = Expression.Equal(member, exp);
                break;
            case QueryOperator.NotEqual:
                condition = Expression.NotEqual(member, exp);
                break;
            case QueryOperator.GreaterThanOrEqual:
                condition = Expression.GreaterThanOrEqual(member, exp);
                break;
            case QueryOperator.LessThanOrEqual:
                condition = Expression.LessThanOrEqual(member, exp);
                break;
            case QueryOperator.GreaterThan:
                condition = Expression.GreaterThan(member, exp);
                break;
            case QueryOperator.LessThan:
                condition = Expression.LessThan(member, exp);
                break;
            default: throw new InvalidOperationException();
        }

        var lambda = Expression.Lambda<Func<T, bool>>(condition, param);
        return lambda;
    }

    /// <summary>
    /// Gets the contains condition.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TValue">The type of the t value.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static Expression<Func<T, bool>> GetContainsCondition<T, TValue>(T source, string propertyName, List<TValue> value)
    {
        var param = Expression.Parameter(typeof(T), "p");
        var methodInfo = typeof(List<TValue>).GetRuntimeMethod("Contains", new[] { typeof(TValue) });
        var list = Expression.Constant(value, typeof(List<TValue>));
        var structure = propertyName.Split('.').ToList();
        var member = SearchMember(param, structure);
        var condition = Expression.Call(list, methodInfo, member);
        var lambda = Expression.Lambda<Func<T, bool>>(condition, param);
        return lambda;
    }

    /// <summary>
    /// Searches the member.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="propertiesName">Name of the properties.</param>
    /// <returns>MemberExpression.</returns>
    private static MemberExpression SearchMember(Expression expression, List<string> propertiesName)
    {
        if (propertiesName.Count != 0)
        {
            expression = Expression.Property(expression, propertiesName.First());
            propertiesName.RemoveAt(0);
        }
        else
        {
            return (MemberExpression)expression;
        }
        return SearchMember(expression, propertiesName);
    }
}
