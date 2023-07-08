using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Nerosoft.Euonia.Business;

public class FieldData<T> : IFieldData<T>
{
    private readonly Stack<T> _histories = new();

    public FieldData()
    {
        ObservableValue.Subscribe(value =>
        {
            _histories.Push(value);
        });
    }

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

    public void MarkAsUnchanged()
    {
        _histories.Clear();
    }

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

    public IObservable<T> ObservableValue => _subject;

    /// <inheritdoc />
    public bool IsChanged => _histories.Count > 0;


}