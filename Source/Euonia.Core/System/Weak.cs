namespace System;

/// <summary>
/// Class Weak.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Weak<T>
{
    /// <summary>
    /// The target
    /// </summary>
    private readonly WeakReference _target;

    /// <summary>
    /// Initializes a new instance of the <see cref="Weak{T}" /> class.
    /// </summary>
    /// <param name="target">The target.</param>
    public Weak(T target)
    {
        _target = new WeakReference(target);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Weak{T}" /> class.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="trackResurrection">if set to <c>true</c> [track resurrection].</param>
    public Weak(T target, bool trackResurrection)
    {
        _target = new WeakReference(target, trackResurrection);
    }

    /// <summary>
    /// Gets or sets the target.
    /// </summary>
    /// <value>The target.</value>
    public T Target
    {
        get => (T)_target.Target;
        set => _target.Target = value;
    }
}