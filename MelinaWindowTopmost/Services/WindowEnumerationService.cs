using System.Diagnostics;
using System.Text;
using MelinaWindowTopmost.Models;
using MelinaWindowTopmost.Native;

namespace MelinaWindowTopmost.Services;

public sealed class WindowEnumerationService
{
    private readonly IconCacheService _iconCache;

    public WindowEnumerationService(IconCacheService iconCache)
    {
        _iconCache = iconCache;
    }

    public Task<IReadOnlyList<WindowInfo>> EnumerateAsync(CancellationToken cancellationToken)
    {
        return Task.Run<IReadOnlyList<WindowInfo>>(() =>
        {
            List<WindowInfo> windows = [];

            NativeMethods.EnumDesktopWindows(IntPtr.Zero, (hWnd, _) =>
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return false;
                }

                WindowInfo? info = TryCreateWindowInfo(hWnd);
                if (info is not null)
                {
                    windows.Add(info);
                }

                return true;
            }, IntPtr.Zero);

            return windows
                .OrderByDescending(window => window.IsTopMost)
                .ThenBy(window => window.Title, StringComparer.CurrentCultureIgnoreCase)
                .ToList();
        }, cancellationToken);
    }

    private WindowInfo? TryCreateWindowInfo(IntPtr hWnd)
    {
        if (!NativeMethods.IsWindowVisible(hWnd))
        {
            return null;
        }

        string title = GetWindowTitle(hWnd);
        if (string.IsNullOrWhiteSpace(title))
        {
            return null;
        }

        NativeMethods.GetWindowThreadProcessId(hWnd, out int processId);
        string processName = string.Empty;
        string processPath = TryGetProcessPath(processId);
        string description = string.Empty;

        try
        {
            using Process process = Process.GetProcessById(processId);
            processName = process.ProcessName;
            if (!string.IsNullOrWhiteSpace(processPath))
            {
                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(processPath);
                description = versionInfo.FileDescription ?? string.Empty;
            }
        }
        catch
        {
            // Some system/elevated processes do not expose metadata to a normal process.
        }

        IntPtr exStyle = NativeMethods.GetWindowLongPtr(hWnd, NativeMethods.GwlExStyle);
        bool isTopMost = (exStyle.ToInt64() & NativeMethods.WsExTopMost) == NativeMethods.WsExTopMost;
        byte alpha = 255;
        bool hasLayeredOpacity = false;

        if (NativeMethods.GetLayeredWindowAttributes(hWnd, out _, out byte layeredAlpha, out uint flags))
        {
            hasLayeredOpacity = (flags & NativeMethods.LwaAlpha) == NativeMethods.LwaAlpha;
            if (hasLayeredOpacity)
            {
                alpha = layeredAlpha;
            }
        }

        return new WindowInfo
        {
            Handle = hWnd,
            Title = title,
            ProcessId = processId,
            ProcessName = processName,
            ProcessPath = processPath,
            Description = description,
            IsTopMost = isTopMost,
            OpacityAlpha = alpha,
            HasLayeredOpacity = hasLayeredOpacity,
            Icon = _iconCache.GetIcon(hWnd, processId, processPath)
        };
    }

    private static string GetWindowTitle(IntPtr hWnd)
    {
        StringBuilder title = new(512);
        _ = NativeMethods.GetWindowText(hWnd, title, title.Capacity);
        return title.ToString();
    }

    private static string TryGetProcessPath(int processId)
    {
        if (processId <= 0)
        {
            return string.Empty;
        }

        IntPtr handle = NativeMethods.OpenProcess(NativeMethods.ProcessQueryLimitedInformation, false, processId);
        if (handle == IntPtr.Zero)
        {
            return string.Empty;
        }

        try
        {
            StringBuilder path = new(1024);
            int size = path.Capacity;
            return NativeMethods.QueryFullProcessImageName(handle, 0, path, ref size)
                ? path.ToString()
                : string.Empty;
        }
        catch
        {
            return string.Empty;
        }
        finally
        {
            NativeMethods.CloseHandle(handle);
        }
    }
}
