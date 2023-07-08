using System.ComponentModel;

namespace Nerosoft.Euonia.Business;

public interface IBusinessObject : IUseBusinessContext, INotifyPropertyChanged, INotifyPropertyChanging
{
    FieldDataManager FieldManager { get; }

    bool FieldExists(IPropertyInfo property);

    object ReadProperty(IPropertyInfo propertyInfo);

    TValue ReadProperty<TValue>(PropertyInfo<TValue> propertyInfo);

    void LoadProperty(IPropertyInfo propertyInfo, object newValue);

    void LoadProperty<TValue>(PropertyInfo<TValue> propertyInfo, TValue newValue);
}