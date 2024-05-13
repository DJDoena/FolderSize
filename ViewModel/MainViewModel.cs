using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using DoenaSoft.AbstractionLayer.Commands;
using DoenaSoft.AbstractionLayer.IOServices;
using DoenaSoft.AbstractionLayer.UIServices;
using DoenaSoft.FolderSize.Model;
using DoenaSoft.FolderSize.ViewModel.Injection;

namespace DoenaSoft.FolderSize.ViewModel
{
    public sealed class MainViewModel : INotifyPropertyChanged
    {
        private readonly IIOServices _ioServices;

        private readonly IUIServices _uiServices;

        private CancellationTokenSource _cancellationTokenSource;

        private readonly RootPathItem _nothingComboBoxItem;

        private readonly RootPathItem _selectFolderComboBoxItem;

        public static string Title => $"Folder Size {Assembly.GetEntryAssembly().GetName().Version}";

        public IEnumerable<RootPathItem> RootPathItems { get; }

        private RootPathItem _selectedRootPathItem;
        public RootPathItem SelectedRootPathItem
        {
            get => _selectedRootPathItem;
            set
            {
                Debug.WriteLine($"setting {value?.DisplayName ?? "null"}");

                if (_selectedRootPathItem != value)
                {
                    _selectedRootPathItem = value;

                    this.OnPropertyChanged();
                }
            }
        }

        private readonly ICommand _rootPathItemSelectionChangedCommand;
        public ICommand RootPathItemSelectionChangedCommand
            => _rootPathItemSelectionChangedCommand;

        public ObservableCollection<NodeViewModel> RootNodes { get; }

        public event EventHandler CloseRequested;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            _nothingComboBoxItem = new("Nothing", RootPathItemType.Nothing);

            _selectFolderComboBoxItem = new("Please select folder", RootPathItemType.SelectFolder);

            var rootPathComboBoxItems = new List<RootPathItem>()
            {
                _nothingComboBoxItem,
                _selectFolderComboBoxItem,
            };

            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }

            _ioServices = IoC.Resolve<IIOServices>();

            var drives = _ioServices.GetDriveInfos(DriveType.Fixed);

            foreach (var drive in drives)
            {
                rootPathComboBoxItems.Add(new($"{drive.DriveLetter} [ {drive.VolumeLabel} ]", RootPathItemType.Drive, drive));
            }

            this.RootPathItems = rootPathComboBoxItems.ToImmutableList();

            _selectedRootPathItem = _nothingComboBoxItem;

            this.RootNodes = new();

            _rootPathItemSelectionChangedCommand = new RelayCommand(this.CreateNodes);

            _uiServices = IoC.Resolve<IUIServices>();
        }

        private void CreateNodes()
        {
            if (this.SelectedRootPathItem == null
                || this.SelectedRootPathItem == _nothingComboBoxItem)
            {
                return;
            }

            _cancellationTokenSource?.Cancel();

            _cancellationTokenSource?.Dispose();

            _cancellationTokenSource = null;

            this.RootNodes.Clear();

            switch (this.SelectedRootPathItem.Type)
            {
                case RootPathItemType.SelectFolder:
                    {
                        this.CreateRootFolderFromSelectedFolder();

                        this.SelectedRootPathItem = _nothingComboBoxItem;

                        return;
                    }
                case RootPathItemType.Drive:
                    {
                        this.CreateRootFolderFromSelectedDrive(this.SelectedRootPathItem.Drive);

                        return;
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        private void CreateRootFolderFromSelectedFolder()
        {
            if (_uiServices.ShowFolderBrowserDialog(new()
            {
                Description = "Select a folder",
                RootFolder = Environment.SpecialFolder.MyComputer,
                ShowNewFolderButton = false,
            }, out var rootFolder))
            {
                this.SetRootFolder(rootFolder);
            }
        }

        private void CreateRootFolderFromSelectedDrive(IDriveInfo drive)
            => this.SetRootFolder(drive.RootFolderName);

        private void SetRootFolder(string rootFolder)
        {
            var modelNode = IoC.Resolve<IFolderSizeModelBuilder>()
                .BuildFolderNode(_ioServices.GetFolderInfo(rootFolder));

            _cancellationTokenSource = new();

            var viewModelNode = new NodeViewModel(modelNode, null, _cancellationTokenSource.Token);

            this.RootNodes.Add(viewModelNode);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}