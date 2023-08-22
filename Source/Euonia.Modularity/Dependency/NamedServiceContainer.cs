using System.Collections.Concurrent;

namespace System;

/// <summary>
/// The named service container.
/// </summary>
/// <typeparam name="TService"></typeparam>
public class NamedServiceContainer<TService> : ConcurrentDictionary<string, Type>
{
}