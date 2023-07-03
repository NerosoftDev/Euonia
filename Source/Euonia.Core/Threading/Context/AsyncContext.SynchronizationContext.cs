namespace Nerosoft.Euonia.Threading;

public sealed partial class AsyncContext
{
    /// <summary>
    /// The <see cref="SynchronizationContext"/> implementation used by <see cref="AsyncContext"/>.
    /// </summary>
    private sealed class AsyncContextSynchronizationContext : SynchronizationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncContextSynchronizationContext"/> class.
        /// </summary>
        /// <param name="context">The async context.</param>
        public AsyncContextSynchronizationContext(AsyncContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Gets the async context.
        /// </summary>
        public AsyncContext Context { get; }

        /// <summary>
        /// Dispatches an asynchronous message to the async context. If all tasks have been completed and the outstanding asynchronous operation count is zero, then this method has undefined behavior.
        /// </summary>
        /// <param name="d">The <see cref="T:System.Threading.SendOrPostCallback"/> delegate to call. May not be <c>null</c>.</param>
        /// <param name="state">The object passed to the delegate.</param>
        public override void Post(SendOrPostCallback d, object state)
        {
            Context.Enqueue(Context._taskFactory.Run(() => d(state)), true);
        }

        /// <summary>
        /// Dispatches an asynchronous message to the async context, and waits for it to complete.
        /// </summary>
        /// <param name="d">The <see cref="T:System.Threading.SendOrPostCallback"/> delegate to call. May not be <c>null</c>.</param>
        /// <param name="state">The object passed to the delegate.</param>
        public override void Send(SendOrPostCallback d, object state)
        {
            if (AsyncContext.Current == Context)
            {
                d(state);
            }
            else
            {
                var task = Context._taskFactory.Run(() => d(state));
                task.WaitAndUnwrapException();
            }
        }

        /// <summary>
        /// Responds to the notification that an operation has started by incrementing the outstanding asynchronous operation count.
        /// </summary>
        public override void OperationStarted()
        {
            Context.OperationStarted();
        }

        /// <summary>
        /// Responds to the notification that an operation has completed by decrementing the outstanding asynchronous operation count.
        /// </summary>
        public override void OperationCompleted()
        {
            Context.OperationCompleted();
        }

        /// <summary>
        /// Creates a copy of the synchronization context.
        /// </summary>
        /// <returns>A new <see cref="T:System.Threading.SynchronizationContext"/> object.</returns>
        public override SynchronizationContext CreateCopy()
        {
            return new AsyncContextSynchronizationContext(Context);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return Context.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance. It is considered equal if it refers to the same underlying async context as this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as AsyncContextSynchronizationContext;
            if (other == null)
                return false;
            return (Context == other.Context);
        }
    }
}