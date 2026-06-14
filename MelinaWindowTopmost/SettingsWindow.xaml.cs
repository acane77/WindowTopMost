using System.Windows;
using MelinaWindowTopmost.Models;
using MelinaWindowTopmost.ViewModels;

namespace MelinaWindowTopmost;

public partial class SettingsWindow : Window
{
    public SettingsWindow(MainWindowViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
        MicaBackdropItem.Visibility = AppBackdropSupport.SupportsMicaAndTabbed
            ? Visibility.Visible
            : Visibility.Collapsed;
        TabbedBackdropItem.Visibility = AppBackdropSupport.SupportsMicaAndTabbed
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
