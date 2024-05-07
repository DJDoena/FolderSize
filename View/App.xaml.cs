using System.Windows;
using DoenaSoft.AbstractionLayer.IOServices;
using DoenaSoft.AbstractionLayer.UIServices;
using DoenaSoft.FolderSize.Model;
using DoenaSoft.FolderSize.ViewModel.Injection;

namespace DoenaSoft.FolderSize.View;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        IoC.RegisterSingleton<IIOServices>(new IOServices());
        IoC.RegisterSingleton<IUIServices>(new WindowUIServices());
        IoC.RegisterSingleton<IFolderSizeModelBuilder>(new FolderSizeModelBuilder());

        (new MainWindow()).Show();
    }
}