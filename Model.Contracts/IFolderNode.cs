using DoenaSoft.AbstractionLayer.IOServices;

namespace DoenaSoft.FolderSize.Model;

public interface IFolderNode
{
    IEnumerable<IFolderNode> Children { get; }

    IFolderInfo Folder { get; }

    string FullName { get; }

    string Name { get; }

    IFolderNode Parent { get; }

    FolderSizeInfo GetSize(CancellationToken token);
}