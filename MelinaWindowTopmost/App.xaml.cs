using System.Windows;
using MelinaWindowTopmost.Services;
using MelinaWindowTopmost.ViewModels;

namespace MelinaWindowTopmost;

public partial class App : System.Windows.Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        IconCacheService iconCache = new();
        LocalizationService localization = new();
        SettingsService settings = new();
        WindowEnumerationService enumeration = new(iconCache);
        WindowControlService control = new();
        MainWindowViewModel viewModel = new(enumeration, control, settings, iconCache, localization);

        MainWindow window = new(viewModel);
        window.Show();
    }
}
