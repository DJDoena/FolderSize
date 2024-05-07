using System.Diagnostics;
using DoenaSoft.AbstractionLayer.IOServices;

namespace DoenaSoft.FolderSize.ViewModel;

[DebuggerDisplay("{" + nameof(DisplayName) + "}")]
public sealed class RootPathItem
{
    public string DisplayName { get; }

    public RootPathItemType Type { get; }

    public IDriveInfo Drive { get; }

    public RootPathItem(string displayName, RootPathItemType type, IDriveInfo drive = null)
    {
        this.DisplayName = displayName;
        this.Type = type;
        this.Drive = drive;
    }
}