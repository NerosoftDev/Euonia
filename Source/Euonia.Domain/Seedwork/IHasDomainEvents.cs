﻿namespace Nerosoft.Euonia.Domain;

/// <summary>
/// Represents the object has domain events.
/// </summary>
public interface IHasDomainEvents
{
    /// <summary>
    /// Gets the attached domain events.
    /// </summary>
    /// <returns></returns>
    IEnumerable<DomainEvent> GetEvents();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="event"></param>
    /// <typeparam name="TEvent"></typeparam>
    void RaiseEvent<TEvent>(TEvent @event)
        where TEvent : DomainEvent;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="event"></param>
    /// <typeparam name="TEvent"></typeparam>
    void RemoveEvent<TEvent>(TEvent @event)
        where TEvent : DomainEvent;

    /// <summary>
    /// 
    /// </summary>
    void ClearEvents();

    /// <summary>
    /// 
    /// </summary>
    void AttachToEvents();
}