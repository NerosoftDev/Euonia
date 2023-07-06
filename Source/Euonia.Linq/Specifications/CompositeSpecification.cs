﻿using System.Linq.Expressions;

namespace Nerosoft.Euonia.Linq;

/// <summary>
/// Base class for composite specifications
/// </summary>
/// <typeparam name="TEntity">Type of entity that check this specification</typeparam>
public sealed class CompositeSpecification<TEntity> : Specification<TEntity>
    where TEntity : class
{
    private readonly List<ISpecification<TEntity>> _specifications = new();

    private readonly PredicateOperator _composeType;

    /// <summary>
    /// Initialize a new instance of <see cref="CompositeSpecification{T}"/>.
    /// </summary>
    /// <param name="composeType">The compose type.</param>
    /// <param name="specifications">The given specifications.</param>
    /// <exception cref="ArgumentException"></exception>
    public CompositeSpecification(PredicateOperator composeType, params ISpecification<TEntity>[] specifications)
    {
        if (specifications == null || specifications.Length < 2)
        {
            throw new ArgumentException("At least 2 specifications.");
        }

        _composeType = composeType;
        if (specifications != null)
        {
            _specifications.AddRange(specifications);
        }
    }

    /// <summary>
    /// Add a specification to composite.
    /// </summary>
    /// <param name="specification"></param>
    /// <returns></returns>
    public CompositeSpecification<TEntity> Add(ISpecification<TEntity> specification)
    {
        _specifications.Add(specification);
        return this;
    }

    /// <summary>
    /// Adds new specification.
    /// </summary>
    /// <param name="specification"></param>
    /// <returns></returns>
    public CompositeSpecification<TEntity> Add(Func<ISpecification<TEntity>> specification)
    {
        _specifications.Add(specification());
        return this;
    }

    /// <summary>
    /// Adds new specification if condition is true.
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="specification"></param>
    /// <returns></returns>
    public CompositeSpecification<TEntity> AddIf(bool condition, ISpecification<TEntity> specification)
    {
        if (condition)
        {
            _specifications.Add(specification);
        }
        return this;
    }

    /// <summary>
    /// Adds new specification if condition is true.
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="specification"></param>
    /// <returns></returns>
    public CompositeSpecification<TEntity> AddIf(bool condition, Func<ISpecification<TEntity>> specification)
    {
        if (condition)
        {
            _specifications.Add(specification());
        }
        return this;
    }

    /// <summary>
    /// Adds new specification if condition is true.
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="specification"></param>
    /// <returns></returns>
    public CompositeSpecification<TEntity> AddIf(Func<bool> condition, ISpecification<TEntity> specification)
    {
        if (condition())
        {
            _specifications.Add(specification);
        }
        return this;
    }

    /// <summary>
    /// Adds new specification if condition is true.
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="specification"></param>
    /// <returns></returns>
    public CompositeSpecification<TEntity> AddIf(Func<bool> condition, Func<ISpecification<TEntity>> specification)
    {
        if (condition())
        {
            _specifications.Add(specification());
        }
        return this;
    }

    /// <summary>
    /// <see cref="ISpecification{T}"/>
    /// </summary>
    /// <returns><see cref="ISpecification{T}"/></returns>
    public override Expression<Func<TEntity, bool>> Satisfy()
    {
        var expressions = _specifications.Select(t => t.Satisfy());
        return expressions.Compose(_composeType);
    }
}
