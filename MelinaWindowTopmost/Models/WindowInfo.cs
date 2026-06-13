using System.Windows.Media;

namespace MelinaWindowTopmost.Models;

public sealed class WindowInfo
{
    public IntPtr Handle { get; init; }

    public string Title { get; init; } = string.Empty;

    public int ProcessId { get; init; }

    public string ProcessName { get; init; } = string.Empty;

    public string ProcessPath { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public bool IsTopMost { get; init; }

    public byte OpacityAlpha { get; init; } = 255;

    public bool HasLayeredOpacity { get; init; }

    public ImageSource? Icon { get; init; }

    public int OpacityPercent => (int)Math.Round(OpacityAlpha / 255.0 * 100);

    public string Subtitle
    {
        get
        {
            string name = !string.IsNullOrWhiteSpace(Description) ? Description : ProcessName;
            if (ProcessId > 0 && !string.IsNullOrWhiteSpace(name))
            {
                return $"{name} (PID: {ProcessId})";
            }

            return ProcessId > 0 ? $"PID: {ProcessId}" : name;
        }
    }
}
