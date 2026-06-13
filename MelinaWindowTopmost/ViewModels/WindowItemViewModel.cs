using CommunityToolkit.Mvvm.ComponentModel;
using MelinaWindowTopmost.Models;

namespace MelinaWindowTopmost.ViewModels;

public sealed partial class WindowItemViewModel : ObservableObject
{
    [ObservableProperty]
    private WindowInfo _info;

    public WindowItemViewModel(WindowInfo info)
    {
        _info = info;
    }

    public IntPtr Handle => Info.Handle;

    public string HandleText => $"0x{Info.Handle.ToInt64():X}";

    public string Title => Info.Title;

    public string Subtitle => Info.Subtitle;

    public string ProcessName => Info.ProcessName;

    public string ProcessPath => Info.ProcessPath;

    public int ProcessId => Info.ProcessId;

    public bool IsTopMost => Info.IsTopMost;

    public int OpacityPercent => Info.OpacityPercent;

    public string OpacityText => $"{OpacityPercent}%";

    public System.Windows.Media.ImageSource? Icon => Info.Icon;

    public void Update(WindowInfo info)
    {
        Info = info;
        OnPropertyChanged(nameof(Handle));
        OnPropertyChanged(nameof(HandleText));
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(Subtitle));
        OnPropertyChanged(nameof(ProcessName));
        OnPropertyChanged(nameof(ProcessPath));
        OnPropertyChanged(nameof(ProcessId));
        OnPropertyChanged(nameof(IsTopMost));
        OnPropertyChanged(nameof(OpacityPercent));
        OnPropertyChanged(nameof(OpacityText));
        OnPropertyChanged(nameof(Icon));
    }
}
