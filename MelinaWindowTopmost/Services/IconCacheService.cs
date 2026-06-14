using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MelinaWindowTopmost.Native;
using Microsoft.Win32;

namespace MelinaWindowTopmost.Services;

public sealed class IconCacheService
{
    private const int IconFingerprintSize = 32;
    private const int IconChannelTolerance = 10;
    private const double IconSimilarityThreshold = 0.95;
    private const int UwpIconPaddingPercent = 10;

    private readonly ConcurrentDictionary<string, ImageSource> _cache = new();
    private readonly bool _isWindows10OrNewer;
    private static readonly Lazy<int[]?> ModernApplicationIconFingerprint = new(CreateModernApplicationIconFingerprint);
    private static readonly Lazy<int[]?> LegacyApplicationIconFingerprint = new(CreateLegacyApplicationIconFingerprint);

    public IconCacheService()
    {
        string? osVersion = Registry.GetValue(
            @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows NT\CurrentVersion",
            "ProductName",
            null) as string;
        _isWindows10OrNewer = osVersion?.IndexOf("10", StringComparison.Ordinal) >= 0 ||
                              osVersion?.IndexOf("11", StringComparison.Ordinal) >= 0;
    }

    public int Count => _cache.Count;

    public void Clear() => _cache.Clear();

    public ImageSource? GetIcon(IntPtr hWnd, Process? process, string processPath)
    {
        string key = CreateKey(hWnd);
        if (_cache.TryGetValue(key, out ImageSource? cached))
        {
            return cached;
        }

        using Bitmap? bitmap = GetIconBitmap(hWnd, process, processPath);
        ImageSource? source = bitmap is null ? null : CreateBitmapSource(bitmap);

        if (source is not null)
        {
            _cache[key] = source;
        }

        return source;
    }

    private static string CreateKey(IntPtr hWnd) => $"hwnd:{hWnd.ToInt64():X}";

    private Bitmap? GetIconBitmap(IntPtr hWnd, Process? process, string processPath)
    {
        bool isApplicationFrameHost = IsApplicationFrameHost(process, processPath);
        if (_isWindows10OrNewer && isApplicationFrameHost)
        {
            Bitmap? bitmapUWP = TryGetUwpIcon(hWnd, process);
            if (bitmapUWP is not null)
            {
                return bitmapUWP;
            }
        }

        Bitmap? windowBitmap = GetWindowIconBitmap(hWnd);
        if (windowBitmap is not null && !IsDefaultApplicationIcon(windowBitmap))
        {
            return windowBitmap;
        }

        if (_isWindows10OrNewer && !isApplicationFrameHost)
        {
            Bitmap? bitmapUWP = TryGetUwpIcon(hWnd, process);
            if (bitmapUWP is not null)
            {
                windowBitmap?.Dispose();
                return bitmapUWP;
            }
        }

        Bitmap? fileBitmap = GetFileIconBitmap(processPath);
        if (fileBitmap is not null)
        {
            windowBitmap?.Dispose();
            return fileBitmap;
        }

        if (windowBitmap is not null)
        {
            return windowBitmap;
        }

        return GetDefaultIconBitmap();
    }

    private static bool IsApplicationFrameHost(Process? process, string processPath)
    {
        string? processName = null;
        try
        {
            processName = process?.ProcessName;
        }
        catch
        {
        }

        if (string.Equals(processName, "ApplicationFrameHost", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        string fileName = Path.GetFileNameWithoutExtension(processPath);
        return string.Equals(fileName, "ApplicationFrameHost", StringComparison.OrdinalIgnoreCase);
    }

    private static Bitmap? GetWindowIconBitmap(IntPtr hWnd)
    {
        IntPtr hIcon = NativeMethods.GetAppIcon(hWnd);
        return hIcon == IntPtr.Zero ? null : CreateBitmapFromIconHandle(hIcon, destroyIcon: false);
    }

    private static Bitmap? GetFileIconBitmap(string processPath)
    {
        if (string.IsNullOrWhiteSpace(processPath) || !File.Exists(processPath))
        {
            return null;
        }

        IntPtr hIcon = NativeMethods.GetFileIcon(processPath);
        if (hIcon != IntPtr.Zero)
        {
            return CreateBitmapFromIconHandle(hIcon, destroyIcon: true);
        }

        hIcon = ExtractIconFromFile(processPath);
        return hIcon == IntPtr.Zero ? null : CreateBitmapFromIconHandle(hIcon, destroyIcon: true);
    }

    private static Bitmap? GetDefaultIconBitmap()
    {
        IntPtr hIcon = NativeMethods.GetModernApplicationIcon();
        return hIcon == IntPtr.Zero ? null : CreateBitmapFromIconHandle(hIcon, destroyIcon: true);
    }

    private static Bitmap CreateBitmapFromIconHandle(IntPtr hIcon, bool destroyIcon)
    {
        try
        {
            using Icon icon = Icon.FromHandle(hIcon);
            return icon.ToBitmap();
        }
        finally
        {
            if (destroyIcon)
            {
                NativeMethods.DestroyIcon(hIcon);
            }
        }
    }

    private static Bitmap? TryGetUwpIcon(IntPtr hWnd, Process? fallbackProcess)
    {
        Bitmap? bitmap = TryGetUwpIconFromWindowProcess(hWnd);
        bitmap ??= TryGetUwpIconFromChildWindowProcesses(hWnd);

        if (bitmap is not null || fallbackProcess is null)
        {
            return bitmap;
        }

        return TryGetUwpIcon(fallbackProcess);
    }

    private static Bitmap? TryGetUwpIconFromWindowProcess(IntPtr hWnd)
    {
        IntPtr hProcess = NativeMethods.GetProcessHandleFromHwnd(hWnd);
        if (hProcess == IntPtr.Zero)
        {
            return null;
        }

        try
        {
            int processId = NativeMethods.GetProcessId(hProcess);
            if (processId <= 0)
            {
                return null;
            }

            return TryGetUwpIcon(processId);
        }
        catch
        {
            return null;
        }
        finally
        {
            NativeMethods.CloseHandle(hProcess);
        }
    }

    private static Bitmap? TryGetUwpIconFromChildWindowProcesses(IntPtr hWnd)
    {
        HashSet<int> processIds = [];

        NativeMethods.EnumChildWindows(hWnd, (childHWnd, _) =>
        {
            NativeMethods.GetWindowThreadProcessId(childHWnd, out int processId);
            if (processId > 0)
            {
                processIds.Add(processId);
            }

            return true;
        }, IntPtr.Zero);

        foreach (int processId in processIds)
        {
            Bitmap? bitmap = TryGetUwpIcon(processId);
            if (bitmap is not null)
            {
                return bitmap;
            }
        }

        return null;
    }

    private static Bitmap? TryGetUwpIcon(Process process)
    {
        try
        {
            return TryGetUwpIcon(process.Id);
        }
        catch
        {
            return null;
        }
    }

    private static Bitmap? TryGetUwpIcon(int processId)
    {
        try
        {
            UWPProcess.AppxPackage appxPackage = UWPProcess.AppxPackage.FromProcess(processId);
            if (appxPackage is null)
            {
                return null;
            }

            UWPProcess.AppxApp? app = GetAppForPackage(appxPackage);
            string? logoPath = GetAppxLogoPath(appxPackage, app);
            if (logoPath is null)
            {
                return null;
            }

            using Bitmap originalBitmap = new(logoPath);
            Bitmap bitmapBackground = new(originalBitmap.Width, originalBitmap.Height);

            if (IsMostlyWhite(originalBitmap, 80))
            {
                FillBackgroundColor(bitmapBackground, GetAppxBackgroundColor(app) ?? System.Drawing.Color.FromArgb(255, 128, 128));
            }

            using (Graphics gr = Graphics.FromImage(bitmapBackground))
            {
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                gr.DrawImage(originalBitmap, GetCenteredPaddedRectangle(originalBitmap.Size));
            }

            return bitmapBackground;
        }
        catch
        {
            return null;
        }
    }

    private static UWPProcess.AppxApp? GetAppForPackage(UWPProcess.AppxPackage appxPackage)
    {
        string? appId = GetAppId(appxPackage.ApplicationUserModelId);
        if (!string.IsNullOrWhiteSpace(appId))
        {
            UWPProcess.AppxApp? app = appxPackage.Apps.FirstOrDefault(candidate =>
                string.Equals(candidate.Id, appId, StringComparison.OrdinalIgnoreCase));
            if (app is not null)
            {
                return app;
            }
        }

        return appxPackage.Apps.FirstOrDefault();
    }

    private static string? GetAppId(string? applicationUserModelId)
    {
        if (string.IsNullOrWhiteSpace(applicationUserModelId))
        {
            return null;
        }

        int separatorIndex = applicationUserModelId.LastIndexOf('!');
        return separatorIndex >= 0 && separatorIndex < applicationUserModelId.Length - 1
            ? applicationUserModelId[(separatorIndex + 1)..]
            : null;
    }

    private static string? GetAppxLogoPath(UWPProcess.AppxPackage appxPackage, UWPProcess.AppxApp? app)
    {
        foreach (string? resourceName in GetAppxLogoResourceNames(appxPackage, app))
        {
            if (string.IsNullOrWhiteSpace(resourceName))
            {
                continue;
            }

            string? logoPath = appxPackage.FindHighestScaleQualifiedImagePath(resourceName);
            if (!string.IsNullOrWhiteSpace(logoPath) && File.Exists(logoPath))
            {
                return logoPath;
            }
        }

        return null;
    }

    private static IEnumerable<string?> GetAppxLogoResourceNames(UWPProcess.AppxPackage appxPackage, UWPProcess.AppxApp? app)
    {
        if (app is not null)
        {
            yield return app.GetStringValue("Square44x44Logo");
            yield return app.SmallLogo;
            yield return app.Square30x30Logo;
            yield return app.Logo;
            yield return app.Square150x150Logo;
            yield return app.GetStringValue("Square71x71Logo");
            yield return app.Square70x70Logo;
            yield return app.Square310x310Logo;
        }

        yield return appxPackage.Logo;
    }

    private static System.Drawing.Color? GetAppxBackgroundColor(UWPProcess.AppxApp? app)
    {
        if (app is null || string.IsNullOrWhiteSpace(app.BackgroundColor))
        {
            return null;
        }

        if (string.Equals(app.BackgroundColor, "transparent", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        try
        {
            return System.Drawing.ColorTranslator.FromHtml(app.BackgroundColor);
        }
        catch
        {
            return null;
        }
    }

    private static Rectangle GetCenteredPaddedRectangle(System.Drawing.Size size)
    {
        int padding = Math.Max(1, Math.Min(size.Width, size.Height) * UwpIconPaddingPercent / 100);
        int availableWidth = Math.Max(1, size.Width - (padding * 2));
        int availableHeight = Math.Max(1, size.Height - (padding * 2));
        double scale = Math.Min(availableWidth / (double)size.Width, availableHeight / (double)size.Height);
        int width = Math.Max(1, (int)Math.Round(size.Width * scale));
        int height = Math.Max(1, (int)Math.Round(size.Height * scale));
        int x = (size.Width - width) / 2;
        int y = (size.Height - height) / 2;

        return new Rectangle(x, y, width, height);
    }

    private static IntPtr ExtractIconFromFile(string processPath)
    {
        if (string.IsNullOrWhiteSpace(processPath) || !File.Exists(processPath))
        {
            return IntPtr.Zero;
        }

        IntPtr[] largeIcons = [IntPtr.Zero];
        IntPtr[] smallIcons = [IntPtr.Zero];
        NativeMethods.ExtractIconEx(processPath, 0, largeIcons, smallIcons, 1);

        IntPtr result = largeIcons[0] == IntPtr.Zero ? smallIcons[0] : largeIcons[0];

        if (result == largeIcons[0] && smallIcons[0] != IntPtr.Zero)
        {
            NativeMethods.DestroyIcon(smallIcons[0]);
        }
        else if (result == smallIcons[0] && largeIcons[0] != IntPtr.Zero)
        {
            NativeMethods.DestroyIcon(largeIcons[0]);
        }

        return result;
    }

    private static bool IsMostlyWhite(Bitmap bitmap, int thresholdPercent = 80)
    {
        int totalPixels = 0;
        int whitePixels = 0;

        BitmapData data = bitmap.LockBits(
            new Rectangle(0, 0, bitmap.Width, bitmap.Height),
            ImageLockMode.ReadOnly,
            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        unsafe
        {
            byte* ptr = (byte*)data.Scan0;
            int stride = data.Stride;

            for (int y = 0; y < bitmap.Height; y++)
            {
                byte* row = ptr + (y * stride);
                for (int x = 0; x < bitmap.Width; x++)
                {
                    int offset = x * 4;
                    byte b = row[offset];
                    byte g = row[offset + 1];
                    byte r = row[offset + 2];
                    byte a = row[offset + 3];

                    if (a == 255)
                    {
                        totalPixels++;
                        if (r == 255 && g == 255 && b == 255)
                        {
                            whitePixels++;
                        }
                    }
                }
            }
        }

        bitmap.UnlockBits(data);

        return totalPixels > 0 && (whitePixels * 100 / totalPixels) >= thresholdPercent;
    }

    private static void FillBackgroundColor(Bitmap bitmap, System.Drawing.Color color)
    {
        BitmapData data = bitmap.LockBits(
            new Rectangle(0, 0, bitmap.Width, bitmap.Height),
            ImageLockMode.WriteOnly,
            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        unsafe
        {
            byte* ptr = (byte*)data.Scan0;
            int stride = data.Stride;

            for (int y = 0; y < bitmap.Height; y++)
            {
                byte* row = ptr + (y * stride);
                for (int x = 0; x < bitmap.Width; x++)
                {
                    int offset = x * 4;
                    row[offset] = color.B;
                    row[offset + 1] = color.G;
                    row[offset + 2] = color.R;
                    row[offset + 3] = color.A;
                }
            }
        }

        bitmap.UnlockBits(data);
    }

    private static bool IsDefaultApplicationIcon(Bitmap bitmap)
    {
        int[] fingerprint = CreateIconFingerprint(bitmap);
        return IsSimilarFingerprint(fingerprint, ModernApplicationIconFingerprint.Value) ||
               IsSimilarFingerprint(fingerprint, LegacyApplicationIconFingerprint.Value);
    }

    private static int[]? CreateModernApplicationIconFingerprint()
    {
        using Bitmap? bitmap = GetDefaultIconBitmap();
        return bitmap is null ? null : CreateIconFingerprint(bitmap);
    }

    private static int[] CreateLegacyApplicationIconFingerprint()
    {
        using Bitmap bitmap = SystemIcons.Application.ToBitmap();
        return CreateIconFingerprint(bitmap);
    }

    private static int[] CreateIconFingerprint(Bitmap bitmap)
    {
        using Bitmap normalized = new(IconFingerprintSize, IconFingerprintSize, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        using (Graphics graphics = Graphics.FromImage(normalized))
        {
            graphics.Clear(System.Drawing.Color.Transparent);
            graphics.DrawImage(bitmap, new Rectangle(0, 0, IconFingerprintSize, IconFingerprintSize));
        }

        int[] pixels = new int[IconFingerprintSize * IconFingerprintSize];
        for (int y = 0; y < IconFingerprintSize; y++)
        {
            for (int x = 0; x < IconFingerprintSize; x++)
            {
                pixels[(y * IconFingerprintSize) + x] = normalized.GetPixel(x, y).ToArgb();
            }
        }

        return pixels;
    }

    private static bool IsSimilarFingerprint(int[] source, int[]? target)
    {
        if (target is null || source.Length != target.Length)
        {
            return false;
        }

        int similarPixels = 0;
        for (int i = 0; i < source.Length; i++)
        {
            System.Drawing.Color sourceColor = System.Drawing.Color.FromArgb(source[i]);
            System.Drawing.Color targetColor = System.Drawing.Color.FromArgb(target[i]);

            if (Math.Abs(sourceColor.A - targetColor.A) <= IconChannelTolerance &&
                Math.Abs(sourceColor.R - targetColor.R) <= IconChannelTolerance &&
                Math.Abs(sourceColor.G - targetColor.G) <= IconChannelTolerance &&
                Math.Abs(sourceColor.B - targetColor.B) <= IconChannelTolerance)
            {
                similarPixels++;
            }
        }

        return similarPixels / (double)source.Length >= IconSimilarityThreshold;
    }

    private static ImageSource CreateBitmapSource(Bitmap bitmap)
    {
        IntPtr hBitmap = bitmap.GetHbitmap();
        try
        {
            BitmapSource source = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(32, 32));
            source.Freeze();
            return source;
        }
        finally
        {
            _ = DeleteObject(hBitmap);
        }
    }

    [System.Runtime.InteropServices.DllImport("gdi32.dll")]
    private static extern bool DeleteObject(IntPtr hObject);
}
