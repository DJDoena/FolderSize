namespace DoenaSoft.FolderSize.Model;

public sealed class FolderSizeInfo
{
    public ulong Size { get; }

    public bool Uncertain { get; }

    public FolderSizeInfo(ulong size, bool uncertain)
    {
        this.Size = size;
        this.Uncertain = uncertain;
    }
}