using System.Windows;
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
}
