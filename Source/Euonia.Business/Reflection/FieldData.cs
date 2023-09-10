using System.Reactive.Subjects;

namespace Nerosoft.Euonia.Business;

/// <summary>
/// The field data.
/// </summary>
/// <typeparam name="T"></typeparam>
public class FieldData<T> : IFieldData<T>
{
    private readonly Stack<T> _histories = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="FieldData{T}"/> class.
    /// </summary>
    public FieldData()
    {
        ObservableValue.Subscribe(value =>
        {
            _histories.Push(value);
        });
    }

    /// <inheritdoc />
    public FieldData(string name)
        : this()
    {
        Name = name;
    }

    /// <inheritdoc />
    public string Name { get; }

    private readonly BehaviorSubject<T> _subject = new(default);

    private T _value;

    /// <inheritdoc />
    public T Value
    {
        get => _subject.Value;
        set
        {
            if (!Equals(_value, value))
            {
                _value = value;
                _subject.OnNext(value);
            }
        }
    }

    /// <inheritdoc />
    public void MarkAsUnchanged()
    {
        _histories.Clear();
    }

    /// <inheritdoc />
    public void Undo()
    {
        if (_histories.Count > 0 && _histories.TryPop(out var value))
        {
            Value = value;
        }
    }

    object IFieldData.Value
    {
        get => Value;
        set => Value = value == null ? default : (T)value;
    }

    /// <summary>
    /// Gets the observable value.
    /// </summary>
    public IObservable<T> ObservableValue => _subject;

    /// <inheritdoc />
    public bool IsChanged => _histories.Count > 0;


}