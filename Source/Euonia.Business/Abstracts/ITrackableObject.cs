namespace Nerosoft.Euonia.Business;

public interface ITrackableObject
{
    bool IsValid { get; }

    bool IsChanged { get; }

    bool IsDeleted { get; }

    bool IsNew { get; }

    bool IsSavable { get; }
}