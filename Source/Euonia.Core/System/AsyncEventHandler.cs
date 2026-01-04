namespace System;

/// <summary>
/// Represents an asynchronous event handler.
/// </summary>
/// <typeparam name="TEventArgs"></typeparam>
public delegate Task AsyncEventHandler<in TEventArgs>(object sender, TEventArgs args);