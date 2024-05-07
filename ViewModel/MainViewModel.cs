using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
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

        private CancellationTokenSource _calcCancellationTokenSource;

        private readonly RootPathItem _nothingComboBoxItem;

        private readonly RootPathItem _selectFolderComboBoxItem;

        public static string Title => $"Folder Size {Assembly.GetEntryAssembly().GetName().Version}";

        public IEnumerable<RootPathItem> RootPathComboBoxItems { get; }

        private RootPathItem _selectedRootPathComboBoxItem;
        public RootPathItem SelectedRootPathComboBoxItem
        {
            get
            {
                Debug.WriteLine($"getting {_selectedRootPathComboBoxItem?.DisplayName ?? "null"}");

                return _selectedRootPathComboBoxItem;
            }
            set
            {
                Debug.WriteLine($"setting {value?.DisplayName ?? "null"}");

                if (_selectedRootPathComboBoxItem != value)
                {
                    Debug.WriteLine($"overwriting {_selectedRootPathComboBoxItem?.DisplayName ?? "null"}");

                    _selectedRootPathComboBoxItem = value;

                    this.OnPropertyChanged();
                }
            }
        }

        private readonly ICommand _rootPathComboBoxItemSelectionChangedCommand;
        public ICommand RootPathComboBoxItemSelectionChangedCommand
            => _rootPathComboBoxItemSelectionChangedCommand;

        public ObservableCollection<NodeViewModel> RootNodes { get; }

        public event EventHandler CloseRequested;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            _ioServices = IoC.Resolve<IIOServices>();

            _uiServices = IoC.Resolve<IUIServices>();

            _nothingComboBoxItem = new("Nothing", RootPathItemType.Nothing);

            _selectFolderComboBoxItem = new("Please select folder", RootPathItemType.SelectFolder);

            var rootPathComboBoxItems = new List<RootPathItem>()
            {
                _nothingComboBoxItem,
                _selectFolderComboBoxItem,
            };

            var drives = _ioServices.GetDriveInfos(System.IO.DriveType.Fixed);

            foreach (var drive in drives)
            {
                rootPathComboBoxItems.Add(new($"{drive.DriveLetter} [ {drive.VolumeLabel} ]", RootPathItemType.Drive, drive));
            }

            this.RootPathComboBoxItems = rootPathComboBoxItems.ToImmutableList();

            _selectedRootPathComboBoxItem = _nothingComboBoxItem;

            this.RootNodes = new();

            _rootPathComboBoxItemSelectionChangedCommand = new ParameterizedRelayCommand(this.CreateNodes);
        }

        private void CreateNodes(object param)
        {
            if (this.SelectedRootPathComboBoxItem == null
                || this.SelectedRootPathComboBoxItem == _nothingComboBoxItem)
            {
                return;
            }

            _calcCancellationTokenSource?.Cancel();

            _calcCancellationTokenSource?.Dispose();

            _calcCancellationTokenSource = null;

            this.RootNodes.Clear();

            switch (this.SelectedRootPathComboBoxItem.Type)
            {
                case RootPathItemType.SelectFolder:
                    {
                        this.CreateRootFolderFromSelectedFolder();

                        this.SelectedRootPathComboBoxItem = _nothingComboBoxItem;

                        return;
                    }
                case RootPathItemType.Drive:
                    {
                        this.CreateRootFolderFromSelectedDrive(this.SelectedRootPathComboBoxItem.Drive);

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
        {
            this.SetRootFolder(drive.RootFolderName);
        }

        private void SetRootFolder(string rootFolder)
        {
            var modelNode = IoC.Resolve<IFolderSizeModelBuilder>().BuildFolderNode(_ioServices.GetFolderInfo(rootFolder));

            _calcCancellationTokenSource = new();

            var viewModelNode = new NodeViewModel(modelNode, null, _calcCancellationTokenSource.Token);

            this.RootNodes.Add(viewModelNode);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}