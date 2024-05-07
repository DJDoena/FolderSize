using DoenaSoft.AbstractionLayer.IOServices;

namespace DoenaSoft.FolderSize.Model;

public sealed class FolderSizeModelBuilder : IFolderSizeModelBuilder
{
    public IFolderNode BuildFolderNode(IFolderInfo folder)
        => new FolderNode(folder);
}