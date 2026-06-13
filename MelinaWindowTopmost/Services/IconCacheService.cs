using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MelinaWindowTopmost.Native;

namespace MelinaWindowTopmost.Services;

public sealed class IconCacheService
{
    private readonly ConcurrentDictionary<string, ImageSource> _cache = new();
    private readonly Lazy<ImageSource?> _defaultIcon;

    public IconCacheService()
    {
        _defaultIcon = new Lazy<ImageSource?>(CreateDefaultIcon);
    }

    public int Count => _cache.Count;

    public void Clear() => _cache.Clear();

    public ImageSource? GetIcon(IntPtr hWnd, int processId, string processPath)
    {
        string key = CreateKey(processId, processPath);
        if (_cache.TryGetValue(key, out ImageSource? cached))
        {
            return cached;
        }

        ImageSource? source = TryGetWindowIcon(hWnd)
            ?? TryExtractFileIcon(processPath)
            ?? _defaultIcon.Value;

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

    private static ImageSource? TryGetWindowIcon(IntPtr hWnd)
    {
        IntPtr icon = NativeMethods.GetAppIcon(hWnd);
        return icon == IntPtr.Zero ? null : CreateImageSourceFromIconHandle(icon, ownsHandle: false);
    }

    private static ImageSource? TryExtractFileIcon(string processPath)
    {
        if (string.IsNullOrWhiteSpace(processPath) || !File.Exists(processPath))
        {
            return null;
        }

        IntPtr[] largeIcons = [IntPtr.Zero];
        IntPtr[] smallIcons = [IntPtr.Zero];

        try
        {
            NativeMethods.ExtractIconEx(processPath, 0, largeIcons, smallIcons, 1);
            IntPtr icon = largeIcons[0] != IntPtr.Zero ? largeIcons[0] : smallIcons[0];
            return icon == IntPtr.Zero ? null : CreateImageSourceFromIconHandle(icon, ownsHandle: true);
        }
        catch
        {
            return null;
        }
        finally
        {
            if (largeIcons[0] != IntPtr.Zero)
            {
                NativeMethods.DestroyIcon(largeIcons[0]);
            }

            if (smallIcons[0] != IntPtr.Zero)
            {
                NativeMethods.DestroyIcon(smallIcons[0]);
            }
        }
    }

    private static ImageSource? CreateDefaultIcon()
    {
        try
        {
            using Icon? icon = SystemIcons.Application;
            using Bitmap bitmap = icon.ToBitmap();
            return CreateBitmapSource(bitmap);
        }
        catch
        {
            return null;
        }
    }

    private static ImageSource? CreateImageSourceFromIconHandle(IntPtr iconHandle, bool ownsHandle)
    {
        try
        {
            ImageSource source = Imaging.CreateBitmapSourceFromHIcon(
                iconHandle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(32, 32));
            source.Freeze();
            return source;
        }
        catch
        {
            return null;
        }
        finally
        {
            if (ownsHandle && iconHandle != IntPtr.Zero)
            {
                NativeMethods.DestroyIcon(iconHandle);
            }
        }
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
