using DoenaSoft.AbstractionLayer.IOServices;

namespace DoenaSoft.FolderSize.Model;

public interface IFolderSizeModelBuilder
{
    IFolderNode BuildFolderNode(IFolderInfo folder);
}