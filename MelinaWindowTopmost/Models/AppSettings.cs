using CommunityToolkit.Mvvm.ComponentModel;

namespace MelinaWindowTopmost.Models;

public sealed partial class AppSettings : ObservableObject
{
    [ObservableProperty]
    private AppLanguage _language = AppLanguage.System;

    [ObservableProperty]
    private AppTheme _theme = AppTheme.System;

    [ObservableProperty]
    private bool _useMica = true;

    [ObservableProperty]
    private bool _autoRefresh = true;

    [ObservableProperty]
    private double _refreshIntervalSeconds = 1.0;
}
