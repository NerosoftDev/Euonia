namespace Nerosoft.Euonia.Business;

public class PropertyInfoList : List<IPropertyInfo>
{
    public PropertyInfoList()
    {
    }

    public PropertyInfoList(IEnumerable<IPropertyInfo> collection)
        : base(collection)
    {
    }

    public bool IsLocked { get; set; }
}