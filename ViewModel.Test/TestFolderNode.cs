using DoenaSoft.AbstractionLayer.IOServices;
using DoenaSoft.FolderSize.Model;

namespace DoenaSoft.FolderSize.ViewModel.Test;

internal sealed class TestFolderNode : IFolderNode
{
    public IEnumerable<IFolderNode> Children
    {
        get
        {
            yield return new TestFolderNode();
        }
    }

    public IFolderInfo Folder => null;

    public string FullName => "FullTest";

    public string Name => "Test";

    public IFolderNode Parent => new TestFolderNode();

    public FolderSizeInfo GetSize(System.Threading.CancellationToken token)
    {
        return new FolderSizeInfo(15, false);
    }
}
