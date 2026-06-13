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
        Bitmap? bitmapUWP = null;
        if (_isWindows10OrNewer && process is not null)
        {
            bitmapUWP = TryGetUwpIcon(process);
        }

        if (bitmapUWP is not null)
        {
            return bitmapUWP;
        }

        Bitmap? windowBitmap = GetWindowIconBitmap(hWnd);
        if (windowBitmap is not null && !IsDefaultApplicationIcon(windowBitmap))
        {
            return windowBitmap;
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

    private static Bitmap? TryGetUwpIcon(Process process)
    {
        try
        {
            UWPProcess.AppxPackage appxPackage = UWPProcess.AppxPackage.FromProcess(process);
            if (appxPackage is null)
            {
                return null;
            }

            string logoPath = appxPackage.FindHighestScaleQualifiedImagePath(appxPackage.Logo);
            if (logoPath is null)
            {
                return null;
            }

            Bitmap originalBitmap = new(logoPath);
            Bitmap bitmapBackground = new(originalBitmap.Width, originalBitmap.Height);

            if (IsMostlyWhite(originalBitmap, 80))
            {
                FillBackgroundColor(bitmapBackground, System.Drawing.Color.FromArgb(255, 128, 128));
            }

            using (Graphics gr = Graphics.FromImage(bitmapBackground))
            {
                gr.DrawImage(originalBitmap, new PointF(0, 0));
            }

            originalBitmap.Dispose();
            return bitmapBackground;
        }
        catch
        {
            return null;
        }
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
