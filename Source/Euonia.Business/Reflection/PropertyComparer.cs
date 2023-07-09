﻿namespace Nerosoft.Euonia.Business;

internal class PropertyComparer : Comparer<IPropertyInfo>
{
    public override int Compare(IPropertyInfo x, IPropertyInfo y)
    {
        return StringComparer.InvariantCulture.Compare(x?.Name, y?.Name);
    }
}