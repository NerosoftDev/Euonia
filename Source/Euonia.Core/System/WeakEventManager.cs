using System.Reflection;
using System.Runtime.CompilerServices;

namespace System;

/// <summary>
/// From <c>https://github.com/xamarin/Xamarin.Forms/blob/main/Xamarin.Forms.Core/WeakEventManager.cs</c>
/// </summary>
/// <remarks>
/// Patch <c>https://github.com/jonathanpeppers/maui/blob/d7b45739b0ffa6fb393321fdddc9317ffdaa1696/src/Core/src/WeakEventManager.cs</c>
/// </remarks>
public sealed class WeakEventManager
{
    private readonly Dictionary<string, List<Subscription>> _eventHandlers = new();

    /// <summary>
    /// Add event handler
    /// </summary>
    /// <param name="handler">The event handler</param>
    /// <param name="eventName">The event name</param>
    /// <typeparam name="TEventArgs">The type of event argument</typeparam>
    /// <exception cref="ArgumentNullException"></exception>
    public void AddEventHandler<TEventArgs>(EventHandler<TEventArgs> handler, [CallerMemberName] string eventName = null)
        where TEventArgs : EventArgs
    {
        if (string.IsNullOrEmpty(eventName))
        {
            throw new ArgumentNullException(nameof(eventName));
        }

        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        AddEventHandler(eventName, handler.Target, handler.GetMethodInfo());
    }

    /// <summary>
    /// Add event handler
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="eventName"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void AddEventHandler(Delegate handler, [CallerMemberName] string eventName = "")
    {
        if (string.IsNullOrEmpty(eventName))
        {
            throw new ArgumentNullException(nameof(eventName));
        }

        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        AddEventHandler(eventName, handler.Target, handler.GetMethodInfo());
    }

    /// <summary>
    /// Raise up event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    /// <param name="eventName"></param>
    public void HandleEvent(object sender, object args, string eventName)
    {
        var handlers = GetEventHandler(eventName);

        foreach (var (subscriber, handler) in handlers)
        {
            handler.Invoke(subscriber, new[] { sender, args });
        }
    }

    /// <summary>
    /// Remove event handler.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="eventName"></param>
    /// <typeparam name="TEventArgs"></typeparam>
    /// <exception cref="ArgumentNullException"></exception>
    public void RemoveEventHandler<TEventArgs>(EventHandler<TEventArgs> handler, [CallerMemberName] string eventName = null)
        where TEventArgs : EventArgs
    {
        if (string.IsNullOrEmpty(eventName))
        {
            throw new ArgumentNullException(nameof(eventName));
        }

        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        RemoveEventHandler(eventName, handler.Target, handler.GetMethodInfo());
    }

    /// <summary>
    /// Remove event handler.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="eventName"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void RemoveEventHandler(Delegate handler, [CallerMemberName] string eventName = "")
    {
        if (string.IsNullOrEmpty(eventName))
        {
            throw new ArgumentNullException(nameof(eventName));
        }

        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        RemoveEventHandler(eventName, handler.Target, handler.GetMethodInfo());
    }

    private void AddEventHandler(string eventName, object handlerTarget, MethodInfo methodInfo)
    {
        if (!_eventHandlers.TryGetValue(eventName, out var targets))
        {
            targets = new List<Subscription>();
            _eventHandlers.Add(eventName, targets);
        }

        if (handlerTarget == null)
        {
            // This event handler is a static method
            targets.Add(new Subscription(null, methodInfo));
            return;
        }

        targets.Add(new Subscription(new WeakReference(handlerTarget), methodInfo));
    }

    private void RemoveEventHandler(string eventName, object handlerTarget, MemberInfo methodInfo)
    {
        if (!_eventHandlers.TryGetValue(eventName, out var subscriptions))
        {
            return;
        }

        for (var n = subscriptions.Count - 1; n >= 0; n--)
        {
            var current = subscriptions[n];

            if (current.Subscriber != null && !current.Subscriber.IsAlive)
            {
                // If not alive, remove and continue
                subscriptions.RemoveAt(n);
                continue;
            }

            if (current.Subscriber?.Target == handlerTarget && current.Handler.Name == methodInfo.Name)
            {
                // Found the match, we can break
                subscriptions.RemoveAt(n);
                break;
            }
        }
    }

    /// <summary>
    /// Add event handler
    /// </summary>
    /// <param name="handler">The event handler</param>
    /// <param name="eventName">The event name</param>
    /// <exception cref="ArgumentNullException"></exception>
    public void AddEventHandler(EventHandler handler, [CallerMemberName] string eventName = null)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            throw new ArgumentNullException(nameof(eventName));
        }

        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        AddEventHandler(eventName, handler.Target, handler.GetMethodInfo());
    }

    #region Extends

    /// <summary>
    /// Gets event handler for specified event.
    /// </summary>
    /// <param name="eventName">The event name.</param>
    /// <returns></returns>
    private List<(object subscriber, MethodInfo handler)> GetEventHandler(string eventName)
    {
        var toRaise = new List<(object subscriber, MethodInfo handler)>();
        var toRemove = new List<Subscription>();

        if (_eventHandlers.TryGetValue(eventName, out var target))
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < target.Count; i++)
            {
                var subscription = target[i];
                var isStatic = subscription.Subscriber == null;
                if (isStatic)
                {
                    // For a static method, we'll just pass null as the first parameter of MethodInfo.Invoke
                    toRaise.Add((null, subscription.Handler));
                    continue;
                }

                var subscriber = subscription.Subscriber.Target;

                if (subscriber == null)
                {
                    // The subscriber was collected, so there's no need to keep this subscription around
                    toRemove.Add(subscription);
                }
                else
                {
                    toRaise.Add((subscriber, subscription.Handler));
                }
            }

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < toRemove.Count; i++)
            {
                var subscription = toRemove[i];
                target.Remove(subscription);
            }
        }

        {
        }
        return toRaise;
    }

    /// <summary>
    /// Raise up event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    /// <param name="eventName"></param>
    /// <typeparam name="TEventArgs"></typeparam>
    public void HandleEvent<TEventArgs>(object sender, TEventArgs args, string eventName)
        where TEventArgs : EventArgs
    {
        var handlers = GetEventHandler(eventName);

        foreach (var (subscriber, handler) in handlers)
        {
            handler.Invoke(subscriber, new[] { sender, args });
        }
    }

    /// <summary>
    /// Raise up event parallel.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    /// <param name="eventName"></param>
    /// <typeparam name="TEventArgs"></typeparam>
    public void HandleEventParallel<TEventArgs>(object sender, TEventArgs args, string eventName)
        where TEventArgs : EventArgs
    {
        var handlers = GetEventHandler(eventName);

        Parallel.ForEach(handlers, (item, _) =>
        {
            try
            {
                item.handler.Invoke(item.subscriber, new[] { sender, args });
            }
            catch (Exception)
            {
                //
            }
        });
    }

    /// <summary>
    /// Raise up event and ignore exception.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    /// <param name="eventName"></param>
    /// <typeparam name="TEventArgs"></typeparam>
    public void HandleEventSafely<TEventArgs>(object sender, TEventArgs args, string eventName)
        where TEventArgs : EventArgs
    {
        var handlers = GetEventHandler(eventName);

        foreach (var (subscriber, handler) in handlers)
        {
            try
            {
                handler.Invoke(subscriber, new[] { sender, args });
            }
            catch (Exception)
            {
                //
            }
        }
    }

    /// <summary>
    /// Remove event handler.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="eventName"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void RemoveEventHandler(EventHandler handler, [CallerMemberName] string eventName = null)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            throw new ArgumentNullException(nameof(eventName));
        }

        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        RemoveEventHandler(eventName, handler.Target, handler.GetMethodInfo());
    }

    public void RemoveEventHandlers()
    {
        _eventHandlers.Clear();
    }

    public void RemoveEventHandlers(string eventName)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            throw new ArgumentNullException(nameof(eventName));
        }

        _eventHandlers.Remove(eventName);
    }

    #endregion

    private readonly struct Subscription : IEquatable<Subscription>
    {
        public Subscription(WeakReference subscriber, MethodInfo handler)
        {
            Subscriber = subscriber;
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public readonly WeakReference Subscriber;
        public readonly MethodInfo Handler;

        public bool Equals(Subscription other) => Subscriber == other.Subscriber && Handler == other.Handler;

        public override bool Equals(object obj) => obj is Subscription other && Equals(other);

        public override int GetHashCode() => Subscriber?.GetHashCode() ?? 0 ^ Handler.GetHashCode();
    }
}