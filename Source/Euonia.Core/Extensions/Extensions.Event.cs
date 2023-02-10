public static partial class Extensions
{
    /// <summary>
    /// Raises given event safely with given arguments.
    /// </summary>
    /// <param name="handler">The event handler</param>
    /// <param name="sender">Source of the event</param>
    public static void CheckAndInvoke(this EventHandler handler, object sender)
    {
        handler.CheckAndInvoke(sender, EventArgs.Empty);
    }

    /// <summary>
    /// Raises given event safely with given arguments.
    /// </summary>
    /// <param name="handler">The event handler</param>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">Event argument</param>
    public static void CheckAndInvoke(this EventHandler handler, object sender, EventArgs e)
    {
        handler?.Invoke(sender, e);
    }

    /// <summary>
    /// Raises given event safely with given arguments.
    /// </summary>
    /// <typeparam name="TEventArgs">Type of the <see cref="EventArgs"/></typeparam>
    /// <param name="handler">The event handler</param>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">Event argument</param>
    public static void CheckAndInvoke<TEventArgs>(this EventHandler<TEventArgs> handler, object sender, TEventArgs e)
        where TEventArgs : EventArgs
    {
        handler?.Invoke(sender, e);
    }
}