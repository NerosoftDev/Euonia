using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Security;
using Nerosoft.Euonia.Reflection;

// ReSharper disable MemberCanBeProtected.Global

// ReSharper disable MemberCanBePrivate.Global

namespace Nerosoft.Euonia.Business;

public abstract partial class BusinessObject : IBusinessObject, IHasRuleCheck, IDisposable
{
    protected readonly WeakEventManager _events = new();

    private BusinessContext _businessContext;

    public BusinessContext BusinessContext
    {
        get => _businessContext;
        set
        {
            _businessContext = value;
            OnBusinessContextSet();
            Initialize();
            InitializeRules();
        }
    }

    public IServiceProvider GetServiceProvider() => BusinessContext?.CurrentServiceProvider;

    protected virtual void OnBusinessContextSet()
    {
    }

    protected virtual void Initialize()
    {
    }

    public event EventHandler ValidationComplete
    {
        add => _events.AddEventHandler(value);
        remove => _events.RemoveEventHandler(value);
    }

    #region IHasRuleCheck implements

    public void RuleCheckComplete(IPropertyInfo property)
    {
        OnPropertyChanged(property);
    }

    public void RuleCheckComplete(string property)
    {
        OnPropertyChanged(property);
    }

    public void AllRulesComplete()
    {
        OnValidationComplete();
    }

    public void SuspendRuleChecking()
    {
        Rules.SuppressRuleChecking = true;
    }

    public void ResumeRuleChecking()
    {
        Rules.SuppressRuleChecking = false;
    }

    public BrokenRuleCollection GetBrokenRules()
    {
        return Rules.BrokenRules;
    }

    #endregion

    #region Rule check

    public virtual bool IsValid => Rules.IsValid;

    private Rules _rules;

    protected Rules Rules
    {
        get
        {
            if (_rules == null)
            {
                _rules = new Rules(this);
            }
            else if (_rules.Target == null)
            {
                _rules.SetTarget(this);
            }

            return _rules;
        }
    }

    protected virtual void OnValidationComplete()
    {
        _events.HandleEvent(this, EventArgs.Empty, nameof(ValidationComplete));
    }

    private void InitializeRules()
    {
        var rules = RuleManager.GetRules(GetType());
        if (rules.Initialized)
        {
            return;
        }

        lock (rules)
        {
            if (rules.Initialized)
            {
                return;
            }

            try
            {
                Rules.AddDataAnnotations();
                AddRules();
                rules.Initialized = true;
            }
            catch (Exception)
            {
                RuleManager.CleanRules(GetType());
                throw;
            }
        }
    }

    protected RuleManager GetRegisteredRules()
    {
        return Rules.RuleManager;
    }

    protected virtual void AddRules()
    {
    }

    protected virtual void CheckPropertyRules(IPropertyInfo property)
    {
        var propertyNames = Rules.CheckRules(property);
        foreach (var name in propertyNames)
        {
            OnPropertyChanged(name);
        }
    }

    #endregion

    #region INotifyPropertyChanged/INotifyPropertyChanging

    /// <summary>
    /// Gets a value indicate if check rule will call on property changed.
    /// </summary>
    protected internal bool CheckRuleOnPropertyChanged { get; } = false;

    public event PropertyChangedEventHandler PropertyChanged;
    public event PropertyChangingEventHandler PropertyChanging;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected virtual void OnPropertyChanged(IPropertyInfo propertyInfo)
    {
        OnPropertyChanged(propertyInfo.Name);
    }

    protected virtual void OnPropertyChanging(string propertyName)
    {
        PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
    }

    protected virtual void OnPropertyChanging(IPropertyInfo propertyInfo)
    {
        OnPropertyChanging(propertyInfo.Name);
    }

    protected virtual void OnPropertyChanged(string name, object value)
    {
    }

    protected virtual void PropertyHasChanged(IPropertyInfo property)
    {
        ChangedProperties.Add(property);
        if (CheckRuleOnPropertyChanged)
        {
            CheckPropertyRules(property);
        }
        else
        {
            OnPropertyChanged(property);
        }
    }

    protected void PropertyHasChanged(string propertyName)
    {
        PropertyHasChanged(FieldManager.GetRegisteredProperty(propertyName));
    }

    protected virtual List<IPropertyInfo> ChangedProperties { get; } = new();

    public virtual bool HasChangedProperties => ChangedProperties.Any();

    #endregion

    #region Property Checks

    protected bool IsBypassingRuleChecks { get; set; }

    private BypassRuleChecksObject InternalBypassRuleChecks { get; set; }

    /// <summary>
    /// By wrapping this property inside Using block
    /// you can set property values on current business object
    /// without raising PropertyChanged events
    /// and checking user rights.
    /// </summary>
    protected internal BypassRuleChecksObject BypassRuleChecks => BypassRuleChecksObject.GetManager(this);

    protected internal class BypassRuleChecksObject : IDisposable
    {
        private BusinessObject _target;
        private static readonly object _lock = new();

        private BypassRuleChecksObject(BusinessObject target)
        {
            _target = target;
            _target.IsBypassingRuleChecks = true;
        }

        #region IDisposable Members

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="dispose">Dispose flag.</param>
        protected virtual void Dispose(bool dispose)
        {
            DeRef();
        }

        /// <summary>
        /// Gets the BypassPropertyChecks object.
        /// </summary>
        /// <param name="target">The business object.</param>
        /// <returns></returns>
        public static BypassRuleChecksObject GetManager(BusinessObject target)
        {
            lock (_lock)
            {
                target.InternalBypassRuleChecks ??= new BypassRuleChecksObject(target);

                target.InternalBypassRuleChecks.AddRef();
            }

            return target.InternalBypassRuleChecks;
        }

        #region Reference counting

        private int _refCount;

        /// <summary>
        /// Gets the current reference count for this
        /// object.
        /// </summary>
        public int RefCount
        {
            get { return _refCount; }
        }

        private void AddRef()
        {
            _refCount += 1;
        }

        private void DeRef()
        {
            lock (_lock)
            {
                _refCount -= 1;
                if (_refCount == 0)
                {
                    _target.IsBypassingRuleChecks = false;
                    _target.InternalBypassRuleChecks = null;
                    _target = null;
                }
            }
        }

        #endregion

        #endregion
    }

    #endregion

    protected static PropertyInfo<TValue> RegisterProperty<TValue>(Type objectType, PropertyInfo<TValue> info)
    {
        return PropertyInfoManager.RegisterProperty(objectType, info);
    }

    #region Fields

    private FieldDataManager _fieldManager;

    public FieldDataManager FieldManager => _fieldManager ??= new FieldDataManager(GetType());

    #endregion

    #region Read Properties

    /// <summary>
    /// Gets a property's value from the list of managed field values, converting the value to an appropriate type.
    /// </summary>
    /// <param name="propertyInfo">PropertyInfo object containing property metadata.</param>
    /// <typeparam name="TValue">Type of the field.</typeparam>
    /// <typeparam name="TProperty">Type of the property.</typeparam>
    /// <returns></returns>
    protected TProperty ReadPropertyConvert<TValue, TProperty>(PropertyInfo<TValue> propertyInfo)
    {
        return TypeHelper.CoerceValue<TProperty>(typeof(TValue), ReadProperty(propertyInfo));
    }

    /// <summary>
    /// Gets a property's value as a specified type.
    /// </summary>
    /// <param name="propertyInfo">PropertyInfo object containing property metadata.</param>
    /// <typeparam name="TValue">Type of the property value.</typeparam>
    /// <returns></returns>
    public TValue ReadProperty<TValue>(PropertyInfo<TValue> propertyInfo)
    {
        TValue result;
        var data = FieldManager.GetFieldData(propertyInfo);
        if (data != null)
        {
            if (data is IFieldData<TValue> fd)
                result = fd.Value;
            else
                result = (TValue)data.Value;
        }
        else
        {
            result = propertyInfo.DefaultValue;
            FieldManager.LoadFieldData(propertyInfo, result);
        }

        return result;
    }

    /// <summary>
    /// Gets a property's value.
    /// </summary>
    /// <param name="propertyInfo">PropertyInfo object containing property metadata.</param>
    /// <returns></returns>
    public virtual object ReadProperty(IPropertyInfo propertyInfo)
    {
        object result;
        var info = FieldManager.GetFieldData(propertyInfo);
        if (info != null)
        {
            result = info.Value;
        }
        else
        {
            result = propertyInfo.DefaultValue;
            FieldManager.LoadFieldData(propertyInfo, result);
        }

        return result;
    }

    #endregion

    #region Load Properties

    public bool FieldExists(IPropertyInfo property)
    {
        return FieldManager.FieldExists(property);
    }

    public void LoadProperty<TValue>(PropertyInfo<TValue> propertyInfo, TValue newValue)
    {
        TValue oldValue;
        var fieldData = FieldManager.GetFieldData(propertyInfo);
        switch (fieldData)
        {
            case null:
                oldValue = propertyInfo.DefaultValue;
                var _ = FieldManager.LoadFieldData(propertyInfo, oldValue);
                break;
            case IFieldData<TValue> fd:
                oldValue = fd.Value;
                break;
            default:
                oldValue = (TValue)fieldData.Value;
                break;
        }

        LoadPropertyValue(propertyInfo, oldValue, newValue, false);
    }

    protected void LoadPropertyValue<TValue>(IPropertyInfo propertyInfo, TValue oldValue, TValue newValue, bool markAsChanged)
    {
        var valuesDiffer = ValuesDiffer(propertyInfo, newValue, oldValue);

        if (!valuesDiffer)
        {
            return;
        }

        if (markAsChanged)
        {
            OnPropertyChanging(propertyInfo);
            FieldManager.SetFieldData(propertyInfo, newValue);
            PropertyHasChanged(propertyInfo);
        }
        else
        {
            FieldManager.LoadFieldData(propertyInfo, newValue);
        }
    }

    public virtual void LoadProperty(IPropertyInfo propertyInfo, object newValue)
    {
#if IOS
        //manually call LoadProperty<T> if the type is nullable otherwise JIT error will occur
        if (propertyInfo.Type == typeof(int?))
        {
            LoadProperty((PropertyInfo<int?>)propertyInfo, (int?)newValue);
        }
        else if (propertyInfo.Type == typeof(bool?))
        {
            LoadProperty((PropertyInfo<bool?>)propertyInfo, (bool?)newValue);
        }
        else if (propertyInfo.Type == typeof(DateTime?))
        {
            LoadProperty((PropertyInfo<DateTime?>)propertyInfo, (DateTime?)newValue);
        }
        else if (propertyInfo.Type == typeof(decimal?))
        {
            LoadProperty((PropertyInfo<decimal?>)propertyInfo, (decimal?)newValue);
        }
        else if (propertyInfo.Type == typeof(double?))
        {
            LoadProperty((PropertyInfo<double?>)propertyInfo, (double?)newValue);
        }
        else if (propertyInfo.Type == typeof(long?))
        {
            LoadProperty((PropertyInfo<long?>)propertyInfo, (long?)newValue);
        }
        else if (propertyInfo.Type == typeof(byte?))
        {
            LoadProperty((PropertyInfo<byte?>)propertyInfo, (byte?)newValue);
        }
        else if (propertyInfo.Type == typeof(char?))
        {
            LoadProperty((PropertyInfo<char?>)propertyInfo, (char?)newValue);
        }
        else if (propertyInfo.Type == typeof(short?))
        {
            LoadProperty((PropertyInfo<short?>)propertyInfo, (short?)newValue);
        }
        else if (propertyInfo.Type == typeof(uint?))
        {
            LoadProperty((PropertyInfo<uint?>)propertyInfo, (uint?)newValue);
        }
        else if (propertyInfo.Type == typeof(ulong?))
        {
            LoadProperty((PropertyInfo<ulong?>)propertyInfo, (ulong?)newValue);
        }
        else if (propertyInfo.Type == typeof(ushort?))
        {
            LoadProperty((PropertyInfo<ushort?>)propertyInfo, (ushort?)newValue);
        }
        else
        {
            LoadPropertyByReflection("LoadProperty", propertyInfo, newValue);
        }
#else
        LoadPropertyByReflection("LoadProperty", propertyInfo, newValue);
#endif
    }

    /// <summary>
    /// Calls the generic LoadProperty method via reflection.
    /// </summary>
    /// <param name="methodName">The LoadProperty method name to call via reflection.</param>
    /// <param name="propertyInfo">PropertyInfo object containing property metadata.</param>
    /// <param name="newValue">The new value for the property.</param>
    /// <returns></returns>
    /// <exception cref="MissingMethodException"></exception>
    private object LoadPropertyByReflection(string methodName, IPropertyInfo propertyInfo, object newValue)
    {
        var t = GetType();
        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        var method = t.GetMethods(flags).FirstOrDefault(c => c.Name == methodName && c.IsGenericMethod);
        if (method == null)
        {
            throw new MissingMethodException(t.FullName, methodName);
        }

        var genericMethod = method.MakeGenericMethod(propertyInfo.Type);
        var parameters = new[] { propertyInfo, newValue };
        return genericMethod.Invoke(this, parameters);
    }

    /// <summary>
    /// Check if old and new values are different.
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="newValue"></param>
    /// <param name="oldValue"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    private static bool ValuesDiffer<TValue>(IPropertyInfo propertyInfo, TValue newValue, TValue oldValue)
    {
        bool valuesDiffer;
        if (oldValue == null)
        {
            valuesDiffer = newValue != null;
        }
        else
        {
            // use reference equals for objects that inherit from CSLA base class
            if (typeof(IBusinessObject).IsAssignableFrom(propertyInfo.Type))
            {
                valuesDiffer = !(ReferenceEquals(oldValue, newValue));
            }
            else
            {
                valuesDiffer = !(oldValue.Equals(newValue));
            }
        }

        return valuesDiffer;
    }

    #endregion

    #region Authorization

    public virtual bool CanReadProperty(IPropertyInfo property)
    {
        return true;
    }

    public bool CanReadProperty(IPropertyInfo property, bool throwOnFalse)
    {
        bool result = CanReadProperty(property);
        if (throwOnFalse && result == false)
        {
            throw new SecurityException($"Property get not allowed. {property.Name}");
        }

        return result;
    }

    public bool CanReadProperty(string propertyName)
    {
        return CanReadProperty(propertyName, false);
    }

    private bool CanReadProperty(string propertyName, bool throwOnFalse)
    {
        var propertyInfo = FieldManager.GetRegisteredProperties().FirstOrDefault(p => p.Name == propertyName);
        if (propertyInfo == null)
        {
            Trace.TraceError("CanReadProperty: {0} is not a registered property of {1}.{2}", propertyName, this.GetType().Namespace, this.GetType().Name);
            return true;
        }

        return CanReadProperty(propertyInfo, throwOnFalse);
    }

    public virtual bool CanWriteProperty(IPropertyInfo property)
    {
        return true;
    }

    public bool CanWriteProperty(IPropertyInfo property, bool throwOnFalse)
    {
        var result = CanWriteProperty(property);
        if (throwOnFalse && result == false)
        {
            throw new SecurityException($"Property set not allowed. {property.Name}");
        }

        return result;
    }

    public bool CanWriteProperty(string propertyName)
    {
        return CanWriteProperty(propertyName, false);
    }

    /// <summary>
    /// Returns true if the user is allowed to write the specified property.
    /// </summary>
    /// <param name="propertyName">Name of the property to write.</param>
    /// <param name="throwOnFalse">Indicates whether a negative result should cause an exception.</param>
    /// <returns><c>True</c> if the user is allowed to write property value, otherwise <c>False</c></returns>
    private bool CanWriteProperty(string propertyName, bool throwOnFalse)
    {
        var propertyInfo = FieldManager.GetRegisteredProperties().FirstOrDefault(p => p.Name == propertyName);
        if (propertyInfo == null)
        {
            Trace.TraceError("CanReadProperty: {0} is not a registered property of {1}.{2}", propertyName, this.GetType().Namespace, this.GetType().Name);
            return true;
        }

        return CanWriteProperty(propertyInfo, throwOnFalse);
    }

    #endregion

    #region IDisposable
    private bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // 释放托管状态(托管对象)
            }

            // 释放未托管的资源(未托管的对象)并重写终结器
            // 将大型字段设置为 null
            disposedValue = true;
        }
    }

    // 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
    ~BusinessObject()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}