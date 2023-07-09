using System;
using System.Collections.Generic;
using System.Text;

namespace Nerosoft.Euonia.Business;

public interface IOperableProperty
{
    object GetProperty(IPropertyInfo propertyInfo);

    void SetProperty(IPropertyInfo propertyInfo, object newValue);
}
