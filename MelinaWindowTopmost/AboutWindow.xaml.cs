using System.Windows;
using System.Windows.Navigation;
using MelinaWindowTopmost.Services;

namespace MelinaWindowTopmost;

public partial class AboutWindow : Window
{
    public AboutWindow(LocalizationService localization)
    {
        DataContext = localization;
        InitializeComponent();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void GitHubRepo_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri)
        {
            UseShellExecute = true
        });
        e.Handled = true;
    }
}
