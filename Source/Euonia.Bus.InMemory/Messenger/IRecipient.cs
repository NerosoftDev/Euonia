﻿namespace Nerosoft.Euonia.Bus.InMemory;

/// <summary>
/// An interface for a recipient that declares a registration for a specific message type.
/// </summary>
/// <typeparam name="TMessage">The type of message to receive.</typeparam>
public interface IRecipient<in TMessage>
	where TMessage : class
{
	/// <summary>
	/// Receives a given <typeparamref name="TMessage"/> message instance.
	/// </summary>
	/// <param name="pack">The message being received.</param>
	void Receive(TMessage pack);
}