using DoenaSoft.AbstractionLayer.IOServices;

namespace DoenaSoft.FolderSize.Model.Test;

internal sealed class TestFolderInfo : IFolderInfo
{
    public IIOServices IOServices { get; }

    public string Name { get; }

    public IFolderInfo Root { get; }

    public IDriveInfo Drive { get; }

    public bool Exists { get; }

    public string FullName { get; }

    public DateTime LastWriteTime { get; set; }

    public DateTime LastWriteTimeUtc { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime CreationTimeUtc { get; set; }

    public DateTime LastAccessTime { get; set; }

    public DateTime LastAccessTimeUtc { get; set; }

    public void Create()
    {
        throw new NotImplementedException();
    }

    public bool Equals(IFolderInfo other)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IFolderInfo> GetDirectories(string searchPattern
        , SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IFileInfo> GetFileInfos(string searchPattern
        , SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IFileInfo> GetFiles(string searchPattern
        , SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IFolderInfo> GetFolderInfos(string searchPattern
        , SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return new List<IFolderInfo>()
        {
            new TestFolderInfo(),
            new TestFolderInfo(),
        };
    }
}