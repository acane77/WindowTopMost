using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MelinaWindowTopmost.ViewModels;
using Wpf.Ui.Controls;

namespace MelinaWindowTopmost;

public partial class MainWindow : FluentWindow
{
    private readonly MainWindowViewModel _viewModel;

    public MainWindow(MainWindowViewModel viewModel)
    {
        _viewModel = viewModel;
        DataContext = _viewModel;
        InitializeComponent();
        ApplyBackdrop();
        Loaded += MainWindow_Loaded;
        Closed += MainWindow_Closed;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.InitializeAsync();
    }

    private void MainWindow_Closed(object? sender, EventArgs e)
    {
        _viewModel.SaveSettings();
        _viewModel.Dispose();
    }

    private void ApplyBackdrop()
    {
        ExtendsContentIntoTitleBar = true;
        WindowBackdropType = _viewModel.Settings.UseMica ? WindowBackdropType.Mica : WindowBackdropType.None;
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        SettingsWindow dialog = new(_viewModel)
        {
            Owner = this
        };

        if (dialog.ShowDialog() == true)
        {
            _viewModel.SaveSettings();
            ApplyBackdrop();
        }
    }

    private void AboutButton_Click(object sender, RoutedEventArgs e)
    {
        AboutWindow dialog = new(_viewModel.Localization)
        {
            Owner = this
        };
        dialog.ShowDialog();
    }

    private void WindowList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (_viewModel.SwitchToCommand.CanExecute(null))
        {
            _viewModel.SwitchToCommand.Execute(null);
        }
    }

    private void WindowList_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        DependencyObject? source = e.OriginalSource as DependencyObject;
        while (source is not null && source is not ListBoxItem)
        {
            source = System.Windows.Media.VisualTreeHelper.GetParent(source);
        }

        if (source is ListBoxItem item)
        {
            item.IsSelected = true;
            item.Focus();
        }
        else
        {
            WindowList.SelectedItem = null;
        }
    }

    private void WindowContextMenu_Opened(object sender, RoutedEventArgs e)
    {
        bool hasSelection = _viewModel.SelectedWindow is not null;
        TopMostMenuItem.IsEnabled = hasSelection && _viewModel.SelectedWindow?.IsTopMost == false;
        CancelTopMostMenuItem.IsEnabled = hasSelection && _viewModel.SelectedWindow?.IsTopMost == true;
    }

    private void SwitchToMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.SwitchToCommand.CanExecute(null))
        {
            _viewModel.SwitchToCommand.Execute(null);
        }
    }

    private void TopMostMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.SetTopMostCommand.CanExecute(null))
        {
            _viewModel.SetTopMostCommand.Execute(null);
        }
    }

    private void CancelTopMostMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.CancelTopMostCommand.CanExecute(null))
        {
            _viewModel.CancelTopMostCommand.Execute(null);
        }
    }

    private void OpacityMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is System.Windows.Controls.MenuItem { Tag: string value } && int.TryParse(value, out int opacity))
        {
            ExecuteOpacityCommand(opacity);
        }
    }

    private void CustomOpacityMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.SelectedWindow is null)
        {
            return;
        }

        OpacityWindow dialog = new(_viewModel.SelectedWindow.OpacityPercent, _viewModel.Localization)
        {
            Owner = this
        };

        if (dialog.ShowDialog() == true)
        {
            ExecuteOpacityCommand(dialog.OpacityPercent);
        }
    }

    private void InformationMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.SelectedWindow is null)
        {
            return;
        }

        InformationWindow dialog = new(_viewModel.SelectedWindow, _viewModel.Localization)
        {
            Owner = this
        };
        dialog.ShowDialog();
    }

    private void OpenLocationMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.OpenLocationCommand.CanExecute(null))
        {
            _viewModel.OpenLocationCommand.Execute(null);
        }
    }

    private void ExecuteOpacityCommand(int opacity)
    {
        int? parameter = opacity;
        if (_viewModel.SetOpacityCommand.CanExecute(parameter))
        {
            _viewModel.SetOpacityCommand.Execute(parameter);
        }
    }
}
