using System.Collections.Immutable;
using System.Diagnostics;
using DoenaSoft.AbstractionLayer.IOServices;

namespace DoenaSoft.FolderSize.Model;

[DebuggerDisplay("{" + nameof(FullName) + "}")]
public sealed class FolderNode : IFolderNode
{
    private readonly object _childLock;

    private readonly object _sizeLock;

    private IEnumerable<FolderNode> _children;

    private Tuple<ulong?> _ownSize;

    private ulong? _summarySize;

    public IFolderInfo Folder { get; }

    public string Name
        => this.Folder.Name;

    public string FullName
        => this.Folder.FullName;

    public IFolderNode Parent { get; private set; }

    private ulong? OwnSize
    {
        get
        {
            lock (_sizeLock)
            {
                _ownSize ??= this.GetCurrentSize();
            }

            return _ownSize.Item1;
        }
    }

    public IEnumerable<IFolderNode> Children
    {
        get
        {
            using var cts = new CancellationTokenSource();

            var children = this.TryGetChildren(cts.Token);

            return children;
        }
    }

    public FolderNode(IFolderInfo folder)
    {
        _childLock = new();

        _sizeLock = new();

        this.Folder = folder;
    }

    public FolderSizeInfo GetSize(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var summarySize = this.GetSummarySize(token);

        token.ThrowIfCancellationRequested();

        var uncertain = this.IsUncertain(token);

        return new(summarySize, uncertain);
    }

    private ulong GetSummarySize(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        lock (_sizeLock)
        {
            if (!_summarySize.HasValue)
            {
                var size = this.OwnSize ?? 0UL;

                foreach (var child in this.TryGetChildren(token))
                {
                    token.ThrowIfCancellationRequested();

                    size += child.GetSummarySize(token);
                }

                _summarySize = size;
            }

            return _summarySize.Value;
        }
    }

    private bool IsUncertain(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        lock (_sizeLock)
        {
            if (this.OwnSize == null)
            {
                return true;
            }

            foreach (var child in this.TryGetChildren(token))
            {
                token.ThrowIfCancellationRequested();

                if (child.IsUncertain(token))
                {
                    return true;
                }
            }

            return false;
        }
    }

    private IEnumerable<FolderNode> TryGetChildren(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        lock (_childLock)
        {
            if (_children == null)
            {
                try
                {
                    _children = this.GetChildren(token);
                }
                catch
                {
                    _children = Enumerable.Empty<FolderNode>();
                }
            }

            return _children;
        }
    }

    private IEnumerable<FolderNode> GetChildren(CancellationToken token)
    {
        var subFolders = this.Folder.GetFolderInfos("*.*", SearchOption.TopDirectoryOnly);

        var children = new List<FolderNode>();

        foreach (var subFolder in subFolders)
        {
            token.ThrowIfCancellationRequested();

            children.Add(new(subFolder)
            {
                Parent = this,
            });
        }

        return children.ToImmutableList();
    }

    public override int GetHashCode()
        => this.FullName.GetHashCode();

    public override bool Equals(object obj)
        => obj is FolderNode other && this.FullName.Equals(other.FullName);

    private Tuple<ulong?> GetCurrentSize()
    {
        try
        {
            var files = this.Folder.GetFiles("*.*", SearchOption.TopDirectoryOnly);

            var fileSize = files.Any()
                ? files
                    .Select(file => file.Length)
                    .Sum()
                : 0UL;

            return new(fileSize);
        }
        catch
        {
            return new(null);
        }
    }
}