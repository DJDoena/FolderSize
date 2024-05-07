using System.Collections.Immutable;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DoenaSoft.FolderSize.Model;

namespace DoenaSoft.FolderSize.ViewModel;

public sealed class NodeViewModel : INotifyPropertyChanged
{
    private readonly IFolderNode _modelNode;

    private readonly IFolderNode _parentNode;

    private readonly CancellationToken _calcCancellationToken;

    private readonly bool _sizeCalculationStarted;

    public string DisplayName { get; }

    private ImmutableList<NodeViewModel> _children;
    public IEnumerable<NodeViewModel> Children
    {
        get
        {
            _children ??= _modelNode.Children
                .Select(modelChild => new NodeViewModel(modelChild, _modelNode, _calcCancellationToken))
                .ToImmutableList();

            return _children;
        }
    }

    private string _size;
    public string Size
    {
        get
        {
            this.CalculateSizeAsync();

            return _size;
        }
        private set
        {
            if (_size != value)
            {
                _size = value;

                this.OnPropertyChanged();
            }
        }
    }

    private double _percentage;
    public double Percentage
    {
        get
        {
            this.CalculateSizeAsync();

            return _percentage;
        }
        private set
        {
            if (_percentage != value)
            {
                _percentage = value;

                this.OnPropertyChanged();
            }
        }
    }

    public NodeViewModel(IFolderNode modelNode
        , IFolderNode parentNode
        , CancellationToken calcCancellationToken)
    {
        _modelNode = modelNode;

        _parentNode = parentNode;

        _sizeCalculationStarted = false;

        this.DisplayName = parentNode != null
            ? _modelNode.Name
            : _modelNode.FullName;

        _size = "...";

        _percentage = 0d;

        _calcCancellationToken = calcCancellationToken;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private Task CalculateSizeAsync()
    {
        if (!_sizeCalculationStarted)
        {
            var task = Task.Run(() =>
            {
                try
                {
                    this.CalculateSize(_calcCancellationToken);
                }
                catch (OperationCanceledException)
                {
                }
            }, _calcCancellationToken);

            return task;
        }
        else
        {
            return Task.CompletedTask;
        }
    }

    private void CalculateSize(CancellationToken token)
    {
        var size = _modelNode.GetSize(token);

        if (token.IsCancellationRequested)
        {
            return;
        }

        var sizeText = FileSizeHelper.FormatFileSize(size.Size);

        var uncertainText = size.Uncertain
            ? "*"
            : string.Empty;

        this.Size = $"{sizeText}{uncertainText}";

        this.Percentage = this.GetPercentage(size, token);
    }

    private double GetPercentage(FolderSizeInfo ownSize, CancellationToken token)
    {
        if (_parentNode != null)
        {
            var parentSize = _parentNode.GetSize(token);

            var percentage = parentSize.Size != 0d
                ? GetPercentage(ownSize, parentSize.Size)
                : 0d;

            return percentage;
        }
        else
        {
            var drive = _modelNode.Folder.Drive;

            var percentage = GetPercentage(ownSize, drive.TotalSpace);

            return percentage;
        }
    }

    private static double GetPercentage(FolderSizeInfo ownSize, ulong totalSize)
        => Math.Round(ownSize.Size * 100d / totalSize, 1, MidpointRounding.AwayFromZero);

    private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}