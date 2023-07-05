using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Nerosoft.Euonia.Validation;

/// <summary>
/// Class ValidatableObject.
/// </summary>
/// <typeparam name="TValue">The type of the t value.</typeparam>
public class ValidatableObject<TValue> : IValidatableObject, INotifyPropertyChanged
{
    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    /// <returns></returns>
    public event PropertyChangedEventHandler PropertyChanged;

    #region Variables

    /// <summary>
    /// The value
    /// </summary>
    private TValue _value;

    /// <summary>
    /// The is valid
    /// </summary>
    private bool _isValid = true;

    /// <summary>
    /// The rules
    /// </summary>
    private readonly List<IObjectValidator<TValue>> _rules = new();

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>The value.</value>
    public TValue Value
    {
        get => _value;
        set
        {
            if (EqualityComparer<TValue>.Default.Equals(_value, value))
            {
                return;
            }

            _value = value;

            RaisePropertyChanged(nameof(Value));
            ValueChangedCallback?.Invoke(Value);

            if (ValidateOnChanged)
            {

                Validate();
            }
        }
    }

    /// <summary>
    /// Returns true if ... is valid.
    /// </summary>
    /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
    public bool IsValid
    {
        get => _isValid;
        private set => SetProperty(ref _isValid, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="Validate"/> method will be call while value changed or not.
    /// </summary>
    public bool ValidateOnChanged { get; set; } = false;

    /// <summary>
    /// Gets the errors.
    /// </summary>
    /// <value>The errors.</value>
    public ObservableCollection<string> Errors { get; } = new();

    /// <summary>
    /// Gets the rules.
    /// </summary>
    /// <value>The rules.</value>
    public IReadOnlyList<IObjectValidator<TValue>> Rules => _rules;

    public Action<TValue> ValueChangedCallback { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Validates this instance.
    /// </summary>
    public void Validate()
    {
        Errors.Clear();
        if (Rules.Count == 0)
        {
            IsValid = true;
            return;
        }

        foreach (var rule in Rules)
        {
            var result = rule.Validate(Value);
            if (!result)
            {
                Errors.Add(rule.Message);
            }
        }

        IsValid = Errors.Count > 0;
    }

    /// <summary>
    /// Withes the validator.
    /// </summary>
    /// <param name="rule">The rule.</param>
    /// <returns>ValidatableObject&lt;TValue&gt;.</returns>
    /// <exception cref="ArgumentNullException">rule</exception>
    public ValidatableObject<TValue> UseValidator(IObjectValidator<TValue> rule)
    {
        if (rule == null)
        {
            throw new ArgumentNullException(nameof(rule));
        }

        _rules.Add(rule);
        return this;
    }

    protected virtual void RaisePropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Set property value.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    /// <param name="field">The property field.</param>
    /// <param name="value">The new property value.</param>
    /// <param name="propertyName">The property name.</param>
    protected virtual void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        field = value;
        RaisePropertyChanged(propertyName);
    }

    /// <summary>
    /// Set property value if value is different with old value.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    /// <param name="field">The property field.</param>
    /// <param name="value">The new property value.</param>
    /// <param name="propertyName">The property name.</param>
    protected virtual void SetPropertyIfNotEquals<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (field == null || !EqualityComparer<T>.Default.Equals(field, value))
        {
            SetProperty(ref field, value, propertyName);
        }
    }
    #endregion
}