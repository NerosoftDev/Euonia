using System.Windows.Input;

namespace Nerosoft.Euonia.Windows;

/// <summary>
/// Class DelegateCommand.
/// Implements the <see cref="ICommand" />
/// Implements the <see cref="System.Windows.Input.ICommand" />
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="System.Windows.Input.ICommand" />
/// <seealso cref="ICommand" />
public class DelegateCommand<T> : ICommand
{
    /// <summary>
    /// The can execute
    /// </summary>
    private readonly Func<T, bool> _canExecute;

    /// <summary>
    /// The execute action
    /// </summary>
    private readonly Action<T> _executeAction;

    /// <summary>
    /// Initializes a new instance of the <see cref="DelegateCommand{T}" /> class.
    /// </summary>
    /// <param name="executeAction">The execute action.</param>
    /// <param name="canExecute">The can execute.</param>
    /// <exception cref="ArgumentNullException">executeAction</exception>
    public DelegateCommand(Action<T> executeAction, Func<T, bool> canExecute = null)
    {
        _executeAction = executeAction ?? throw new ArgumentNullException(nameof(executeAction));
        _canExecute = canExecute;
    }

    /// <summary>
    /// Defines the method that determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
    /// <returns>true if this command can be executed; otherwise, false.</returns>
    public bool CanExecute(object parameter)
    {
        var result = true;
        var canExecuteHandler = _canExecute;
        if (canExecuteHandler != null)
        {
            result = canExecuteHandler((T)parameter);
        }

        return result;
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
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Defines the method to be called when the command is invoked.
    /// </summary>
    /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
    public void Execute(object parameter)
    {
        _executeAction((T)parameter);
    }
}

/// <summary>
/// Class DelegateCommand.
/// Implements the <see cref="ICommand" />
/// Implements the <see cref="System.Windows.Input.ICommand" />
/// </summary>
/// <seealso cref="System.Windows.Input.ICommand" />
/// <seealso cref="ICommand" />
public class DelegateCommand : ICommand
{
    /// <summary>
    /// The can execute
    /// </summary>
    private readonly Func<object, bool> _canExecute;

    /// <summary>
    /// The execute action
    /// </summary>
    private readonly Action<object> _executeAction;

    /// <summary>
    /// Initializes a new instance of the <see cref="DelegateCommand" /> class.
    /// </summary>
    /// <param name="executeAction">The execute action.</param>
    /// <param name="canExecute">The can execute.</param>
    /// <exception cref="ArgumentNullException">executeAction</exception>
    public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecute = null)
    {
        _executeAction = executeAction ?? throw new ArgumentNullException(nameof(executeAction));
        _canExecute = canExecute;
    }

    /// <summary>
    /// Defines the method that determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
    /// <returns>true if this command can be executed; otherwise, false.</returns>
    public bool CanExecute(object parameter)
    {
        var result = true;
        var canExecuteHandler = _canExecute;
        if (canExecuteHandler != null)
        {
            result = canExecuteHandler(parameter);
        }

        return result;
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
        _executeAction(parameter);
    }
}