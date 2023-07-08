using Nerosoft.Euonia.Reflection;

namespace Nerosoft.Euonia.Business;

public class ReadOnlyObject<T> : BusinessObject<T>, IReadOnlyObject, IOperableProperty
    where T : ReadOnlyObject<T>
{
    #region Get Properties

    /// <summary>
    /// Gets a property's value, first checking authorization.
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="field"></param>
    /// <param name="defaultValue"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    protected TValue GetProperty<TValue>(string propertyName, TValue field, TValue defaultValue)
    {
        #region Check to see if the property is marked with RelationshipTypes.PrivateField

        var propertyInfo = FieldManager.GetRegisteredProperty(propertyName);

        #endregion

        if (IsBypassingRuleChecks || CanReadProperty(propertyInfo, true))
        {
            return field;
        }

        return defaultValue;
    }

    protected TValue GetProperty<TValue>(PropertyInfo<TValue> propertyInfo, TValue field)
    {
        return GetProperty(propertyInfo.Name, field, propertyInfo.DefaultValue);
    }

    protected TValue GetProperty<TValue>(PropertyInfo<TValue> propertyInfo, TValue field, TValue defaultValue)
    {
        return GetProperty(propertyInfo.Name, field, defaultValue);
    }

    protected TValue GetPropertyConvert<TField, TValue>(PropertyInfo<TField> propertyInfo, TField field)
    {
        return TypeHelper.CoerceValue<TValue>(typeof(TField), GetProperty(propertyInfo.Name, field, propertyInfo.DefaultValue));
    }

    protected TValue GetPropertyConvert<TField, TValue>(PropertyInfo<TField> propertyInfo)
    {
        return TypeHelper.CoerceValue<TValue>(typeof(TField), GetProperty(propertyInfo));
    }

    protected TValue GetProperty<TValue>(PropertyInfo<TValue> propertyInfo)
    {
        TValue result;
        if (IsBypassingRuleChecks || CanReadProperty(propertyInfo, true))
            result = ReadProperty(propertyInfo);
        else
            result = propertyInfo.DefaultValue;
        return result;
    }

    public object GetProperty(IPropertyInfo propertyInfo)
    {
        object result;
        if (IsBypassingRuleChecks || CanReadProperty(propertyInfo, false))
        {
            // call ReadProperty (may be overloaded in actual class)
            result = ReadProperty(propertyInfo);
        }
        else
        {
            result = propertyInfo.DefaultValue;
        }

        return result;
    }

    protected TValue GetProperty<TValue>(IPropertyInfo propertyInfo)
    {
        return (TValue)GetProperty(propertyInfo);
    }

    #endregion

    #region Set Properties

    protected void SetProperty<TValue>(PropertyInfo<TValue> propertyInfo, ref TValue field, TValue newValue)
    {
        SetProperty(propertyInfo.Name, ref field, newValue);
    }

    protected void SetPropertyConvert<TField, TValue>(PropertyInfo<TField> propertyInfo, ref TField field, TValue newValue)
    {
        SetPropertyConvert(propertyInfo.Name, ref field, newValue);
    }

    protected void SetProperty<TValue>(string propertyName, ref TValue field, TValue newValue)
    {
        #region Check to see if the property is marked with RelationshipTypes.PrivateField

        var propertyInfo = FieldManager.GetRegisteredProperty(propertyName);

        #endregion

        if (!IsBypassingRuleChecks && !CanWriteProperty(propertyInfo, true))
        {
            return;
        }

        var doChange = false;
        if (field == null)
        {
            if (newValue != null)
                doChange = true;
        }
        else
        {
            if (typeof(TValue) == typeof(string) && newValue == null)
                newValue = TypeHelper.CoerceValue<TValue>(typeof(string), string.Empty);
            if (!field.Equals(newValue))
                doChange = true;
        }

        if (doChange)
        {
            if (!IsBypassingRuleChecks)
            {
                OnPropertyChanging(propertyName);
            }

            field = newValue;
            if (!IsBypassingRuleChecks)
            {
                PropertyHasChanged(propertyName);
            }
        }
    }

    protected void SetPropertyConvert<TField, TValue>(string propertyName, ref TField field, TValue newValue)
    {
        #region Check to see if the property is marked with RelationshipTypes.PrivateField

        var propertyInfo = FieldManager.GetRegisteredProperty(propertyName);

        #endregion

        if (!IsBypassingRuleChecks && !CanWriteProperty(propertyInfo, true))
        {
            return;
        }

        var doChange = false;
        if (field == null)
        {
            if (newValue != null)
            {
                doChange = true;
            }
        }
        else
        {
            if (typeof(TValue) == typeof(string) && newValue == null)
            {
                newValue = TypeHelper.CoerceValue<TValue>(typeof(string), string.Empty);
            }

            if (!field.Equals(newValue))
            {
                doChange = true;
            }
        }

        if (doChange)
        {
            if (!IsBypassingRuleChecks)
            {
                OnPropertyChanging(propertyName);
            }

            field = TypeHelper.CoerceValue<TField>(typeof(TValue), newValue);
            if (!IsBypassingRuleChecks)
            {
                PropertyHasChanged(propertyName);
            }
        }
    }

    protected void SetPropertyConvert<TField, TValue>(PropertyInfo<TField> propertyInfo, TValue newValue)
    {
        if (!IsBypassingRuleChecks && !CanWriteProperty(propertyInfo, true))
        {
            return;
        }

        TField oldValue;
        var fieldData = FieldManager.GetFieldData(propertyInfo);
        switch (fieldData)
        {
            case null:
                oldValue = propertyInfo.DefaultValue;
                var _ = FieldManager.LoadFieldData(propertyInfo, oldValue);
                break;
            case IFieldData<TField> fd:
                oldValue = fd.Value;
                break;
            default:
                oldValue = (TField)fieldData.Value;
                break;
        }

        if (typeof(TValue) == typeof(string) && newValue == null)
        {
            newValue = TypeHelper.CoerceValue<TValue>(typeof(string), string.Empty);
        }

        LoadPropertyValue(propertyInfo, oldValue, TypeHelper.CoerceValue<TField>(typeof(TValue), newValue), !IsBypassingRuleChecks);
    }

    protected void SetProperty<TValue>(PropertyInfo<TValue> propertyInfo, TValue newValue)
    {
        if (!IsBypassingRuleChecks && !CanWriteProperty(propertyInfo, true))
        {
            return;
        }

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

        if (typeof(TValue) == typeof(string) && newValue == null)
        {
            newValue = TypeHelper.CoerceValue<TValue>(typeof(string), string.Empty);
        }

        LoadPropertyValue(propertyInfo, oldValue, newValue, !IsBypassingRuleChecks);
    }

    public void SetProperty(IPropertyInfo propertyInfo, object newValue)
    {
        if (!IsBypassingRuleChecks && !CanWriteProperty(propertyInfo, true))
        {
            return;
        }

        if (!IsBypassingRuleChecks)
        {
            OnPropertyChanging(propertyInfo);
        }

        FieldManager.SetFieldData(propertyInfo, newValue);

        if (!IsBypassingRuleChecks)
        {
            PropertyHasChanged(propertyInfo);
        }
    }

    protected virtual void SetProperty<TValue>(IPropertyInfo propertyInfo, TValue newValue)
    {
        SetProperty(propertyInfo, (object)newValue);
    }

    #endregion
}