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
    private readonly ConcurrentDictionary<string, ImageSource> _cache = new();
    private readonly bool _isWindows10OrNewer;

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

    public ImageSource? GetIcon(IntPtr hWnd, Process? process, int processId, string processPath)
    {
        string key = CreateKey(processId, processPath);
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

    private static string CreateKey(int processId, string processPath)
    {
        if (!string.IsNullOrWhiteSpace(processPath) && File.Exists(processPath))
        {
            DateTime lastWrite = File.GetLastWriteTimeUtc(processPath);
            return $"{processPath}|{lastWrite.Ticks}";
        }

        return $"pid:{processId}";
    }

    private Bitmap? GetIconBitmap(IntPtr hWnd, Process? process, string processPath)
    {
        Bitmap? bitmapUWP = null;
        if (_isWindows10OrNewer && process is not null)
        {
            bitmapUWP = TryGetUwpIcon(process);
        }

        IntPtr hIcon = NativeMethods.GetAppIcon(hWnd);
        Bitmap? bitmap = null;

        if (hIcon != IntPtr.Zero)
        {
            using Icon icon = Icon.FromHandle(hIcon);
            bitmap = icon.ToBitmap();
        }
        else
        {
            IntPtr sysIcon = NativeMethods.GetModernApplicationIcon();
            if (sysIcon != IntPtr.Zero)
            {
                using Icon icon = Icon.FromHandle(sysIcon);
                bitmap = icon.ToBitmap();
                NativeMethods.DestroyIcon(sysIcon);
            }
        }

        if (bitmap == null && hIcon == IntPtr.Zero)
        {
            hIcon = ExtractIconFromFile(processPath);
            if (hIcon != IntPtr.Zero)
            {
                using Icon icon = Icon.FromHandle(hIcon);
                bitmap = icon.ToBitmap();
                NativeMethods.DestroyIcon(hIcon);
            }
        }

        return bitmapUWP ?? bitmap;
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
