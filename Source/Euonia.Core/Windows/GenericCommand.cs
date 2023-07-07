using System.Windows.Input;

namespace Nerosoft.Euonia.Windows;

/// <summary>
/// GenericCommand.
/// Implements the <see cref="ICommand" />
/// Implements the <see cref="System.Windows.Input.ICommand" />
/// </summary>
/// <seealso cref="System.Windows.Input.ICommand" />
/// <seealso cref="ICommand" />
public sealed class GenericCommand : ICommand
{
    /// <summary>
    /// Gets or sets the can execute callback.
    /// </summary>
    /// <value>The can execute callback.</value>
    public Func<object, bool> CanExecuteCallback { get; set; }

    /// <summary>
    /// Gets or sets the execute callback.
    /// </summary>
    /// <value>The execute callback.</value>
    public Action<object> ExecuteCallback { get; set; }

    #region ICommand Members

    /// <summary>
    /// Defines the method that determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
    /// <returns>true if this command can be executed; otherwise, false.</returns>
    public bool CanExecute(object parameter)
    {
        if (CanExecuteCallback != null)
        {
            return CanExecuteCallback(parameter);
        }

        return true;
    }

    /// <summary>
    /// Occurs when changes occur that affect whether or not the command should execute.
    /// </summary>
    /// <returns></returns>
    public event EventHandler CanExecuteChanged;

    /// <summary>
    /// Raises the can execute changed.
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        var handler = CanExecuteChanged;
        handler?.Invoke(this, new EventArgs());
    }

    /// <summary>
    /// Defines the method to be called when the command is invoked.
    /// </summary>
    /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
    public void Execute(object parameter)
    {
        ExecuteCallback?.Invoke(parameter);
    }

    #endregion
}

/// <summary>
/// GenericCommand with specified parameter type.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class GenericCommand<T> : ICommand
{
    /// <summary>
    /// Gets or sets the can execute callback.
    /// </summary>
    /// <value>The can execute callback.</value>
    public Func<T, bool> CanExecuteCallback { get; set; }

    /// <summary>
    /// Gets or sets the execute callback.
    /// </summary>
    /// <value>The execute callback.</value>
    public Action<T> ExecuteCallback { get; set; }

    #region ICommand Members

    /// <summary>
    /// Defines the method that determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
    /// <returns>true if this command can be executed; otherwise, false.</returns>
    public bool CanExecute(object parameter)
    {
        return CanExecuteCallback?.Invoke((T)parameter) ?? true;
    }

    /// <summary>
    /// Occurs when changes occur that affect whether or not the command should execute.
    /// </summary>
    /// <returns></returns>
    public event EventHandler CanExecuteChanged;

    /// <summary>
    /// Raises the can execute changed.
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        var handler = CanExecuteChanged;
        handler?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Defines the method to be called when the command is invoked.
    /// </summary>
    /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
    public void Execute(object parameter)
    {
        ExecuteCallback?.Invoke((T)parameter);
    }

    #endregion
}