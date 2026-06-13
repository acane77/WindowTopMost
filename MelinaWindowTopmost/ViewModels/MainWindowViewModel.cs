using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MelinaWindowTopmost.Models;
using MelinaWindowTopmost.Services;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace MelinaWindowTopmost.ViewModels;

public sealed partial class MainWindowViewModel : ObservableObject, IDisposable
{
    private readonly WindowEnumerationService _enumerationService;
    private readonly WindowControlService _windowControlService;
    private readonly SettingsService _settingsService;
    private readonly IconCacheService _iconCacheService;
    private readonly LocalizationService _localization;
    private CancellationTokenSource? _autoRefreshCts;
    private bool _isDisposed;

    public ObservableCollection<WindowItemViewModel> Windows { get; } = [];

    public ObservableCollection<WindowItemViewModel> FilteredWindows { get; } = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SetTopMostCommand))]
    [NotifyCanExecuteChangedFor(nameof(CancelTopMostCommand))]
    [NotifyCanExecuteChangedFor(nameof(SwitchToCommand))]
    [NotifyCanExecuteChangedFor(nameof(OpenLocationCommand))]
    [NotifyCanExecuteChangedFor(nameof(SetOpacityCommand))]
    [NotifyCanExecuteChangedFor(nameof(ShowInformationCommand))]
    private WindowItemViewModel? _selectedWindow;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private string _statusText = string.Empty;

    [ObservableProperty]
    private AppSettings _settings;

    public MainWindowViewModel(
        WindowEnumerationService enumerationService,
        WindowControlService windowControlService,
        SettingsService settingsService,
        IconCacheService iconCacheService,
        LocalizationService localization)
    {
        _enumerationService = enumerationService;
        _windowControlService = windowControlService;
        _settingsService = settingsService;
        _iconCacheService = iconCacheService;
        _localization = localization;
        Settings = _settingsService.Load();
        Settings.PropertyChanged += Settings_PropertyChanged;
        _localization.Apply(Settings.Language);
        ApplyTheme();
        StatusText = string.Empty;
    }

    public LocalizationService Localization => _localization;

    public string T(string key) => _localization.Translate(key);

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    partial void OnSelectedWindowChanged(WindowItemViewModel? value)
    {
        StatusText = value?.Title ?? string.Empty;
    }

    partial void OnSettingsChanged(AppSettings value)
    {
        value.PropertyChanged += Settings_PropertyChanged;
        RestartAutoRefresh();
    }

    [RelayCommand]
    public async Task InitializeAsync()
    {
        await RefreshAsync();
        RestartAutoRefresh();
    }

    [RelayCommand]
    public async Task RefreshAsync()
    {
        if (IsRefreshing)
        {
            return;
        }

        try
        {
            IsRefreshing = true;
            IntPtr? selectedHandle = SelectedWindow?.Handle;
            IReadOnlyList<WindowInfo> refreshed = await _enumerationService.EnumerateAsync(CancellationToken.None);
            MergeWindows(refreshed);
            ApplyFilter();

            if (selectedHandle.HasValue)
            {
                SelectedWindow = FilteredWindows.FirstOrDefault(item => item.Handle == selectedHandle.Value);
            }
        }
        catch
        {
            StatusText = T("RefreshFailed");
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private async Task SetTopMostAsync()
    {
        await RunWindowActionAsync(() => _windowControlService.SetTopMost(SelectedWindow!.Info, true));
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private async Task CancelTopMostAsync()
    {
        await RunWindowActionAsync(() => _windowControlService.SetTopMost(SelectedWindow!.Info, false));
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private void SwitchTo()
    {
        RunWindowAction(() => _windowControlService.SwitchTo(SelectedWindow!.Info));
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private void OpenLocation()
    {
        RunWindowAction(() => _windowControlService.OpenProcessLocation(SelectedWindow!.Info));
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private async Task SetOpacityAsync(int? opacityPercent)
    {
        if (!opacityPercent.HasValue)
        {
            return;
        }

        await RunWindowActionAsync(() => _windowControlService.SetOpacity(SelectedWindow!.Info, opacityPercent.Value));
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private void ShowInformation()
    {
        if (SelectedWindow is null)
        {
            return;
        }

        string message =
            $"{T("Handle")}: {SelectedWindow.HandleText}{Environment.NewLine}" +
            $"{T("Title")}: {SelectedWindow.Title}{Environment.NewLine}" +
            $"{T("Process")}: {SelectedWindow.ProcessName}{Environment.NewLine}" +
            $"{T("Pid")}: {SelectedWindow.ProcessId}{Environment.NewLine}" +
            $"{T("Path")}: {SelectedWindow.ProcessPath}";

        System.Windows.MessageBox.Show(message, T("Information"), System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
    }

    [RelayCommand]
    private void ClearIconCache()
    {
        _iconCacheService.Clear();
        StatusText = T("CacheCleared");
    }

    public void SaveSettings()
    {
        Settings.RefreshIntervalSeconds = Math.Clamp(Settings.RefreshIntervalSeconds, 0.5, 10.0);
        _settingsService.Save(Settings);
        _localization.Apply(Settings.Language);
        ApplyTheme();
        OnPropertyChanged(nameof(Localization));
        RestartAutoRefresh();
    }

    private void Settings_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(AppSettings.AutoRefresh) or nameof(AppSettings.RefreshIntervalSeconds))
        {
            RestartAutoRefresh();
        }
    }

    private void ApplyTheme()
    {
        ApplicationTheme theme = Settings.Theme switch
        {
            AppTheme.Dark => ApplicationTheme.Dark,
            AppTheme.Light => ApplicationTheme.Light,
            _ => ApplicationThemeManager.GetSystemTheme() == SystemTheme.Dark
                ? ApplicationTheme.Dark
                : ApplicationTheme.Light
        };

        ApplicationThemeManager.Apply(theme, WindowBackdropType.None, updateAccent: true);
    }

    private bool HasSelection() => SelectedWindow is not null;

    private async Task RunWindowActionAsync(Action action)
    {
        RunWindowAction(action);
        await RefreshAsync();
    }

    private void RunWindowAction(Action action)
    {
        try
        {
            action();
        }
        catch (InvalidOperationException ex)
        {
            System.Windows.MessageBox.Show(ex.Message, T("AppName"), System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
        }
        catch
        {
            System.Windows.MessageBox.Show(T("ActionFailed"), T("AppName"), System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    private void MergeWindows(IReadOnlyList<WindowInfo> refreshed)
    {
        Dictionary<IntPtr, WindowItemViewModel> existing = Windows.ToDictionary(item => item.Handle);
        Windows.Clear();

        foreach (WindowInfo info in refreshed)
        {
            if (existing.TryGetValue(info.Handle, out WindowItemViewModel? item))
            {
                item.Update(info);
                Windows.Add(item);
            }
            else
            {
                Windows.Add(new WindowItemViewModel(info));
            }
        }
    }

    private void ApplyFilter()
    {
        string query = SearchText.Trim();
        IEnumerable<WindowItemViewModel> filtered = Windows;

        if (!string.IsNullOrWhiteSpace(query))
        {
            filtered = filtered.Where(item =>
                item.Title.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                item.Subtitle.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                item.ProcessName.Contains(query, StringComparison.CurrentCultureIgnoreCase));
        }

        FilteredWindows.Clear();
        foreach (WindowItemViewModel item in filtered)
        {
            FilteredWindows.Add(item);
        }
    }

    private void RestartAutoRefresh()
    {
        _autoRefreshCts?.Cancel();
        _autoRefreshCts?.Dispose();
        _autoRefreshCts = null;

        if (!Settings.AutoRefresh || _isDisposed)
        {
            return;
        }

        _autoRefreshCts = new CancellationTokenSource();
        _ = RunAutoRefreshLoopAsync(_autoRefreshCts.Token);
    }

    private async Task RunAutoRefreshLoopAsync(CancellationToken cancellationToken)
    {
        double seconds = Math.Clamp(Settings.RefreshIntervalSeconds, 0.5, 10.0);
        using PeriodicTimer timer = new(TimeSpan.FromSeconds(seconds));

        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                await RefreshAsync();
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    public void Dispose()
    {
        _isDisposed = true;
        Settings.PropertyChanged -= Settings_PropertyChanged;
        _autoRefreshCts?.Cancel();
        _autoRefreshCts?.Dispose();
    }
}
