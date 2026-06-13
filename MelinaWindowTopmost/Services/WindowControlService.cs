using System.ComponentModel;
using System.Diagnostics;
using MelinaWindowTopmost.Models;
using MelinaWindowTopmost.Native;

namespace MelinaWindowTopmost.Services;

public sealed class WindowControlService
{
    public void SetTopMost(WindowInfo window, bool topMost)
    {
        IntPtr insertAfter = topMost ? NativeMethods.HwndTopMost : NativeMethods.HwndNoTopMost;
        bool success = NativeMethods.SetWindowPos(
            window.Handle,
            insertAfter,
            0,
            0,
            0,
            0,
            NativeMethods.SwpNoMove | NativeMethods.SwpNoSize | NativeMethods.SwpNoActivate | NativeMethods.SwpShowWindow);

        if (!success)
        {
            throw new Win32Exception("Unable to change the target window topmost state.");
        }
    }

    public void SwitchTo(WindowInfo window)
    {
        NativeMethods.SwitchToThisWindow(window.Handle, true);
    }

    public void OpenProcessLocation(WindowInfo window)
    {
        if (string.IsNullOrWhiteSpace(window.ProcessPath))
        {
            throw new InvalidOperationException("The selected window does not expose a process path.");
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = "explorer.exe",
            Arguments = $"/select,\"{window.ProcessPath}\"",
            UseShellExecute = true
        });
    }

    public void SetOpacity(WindowInfo window, int opacityPercent)
    {
        opacityPercent = Math.Clamp(opacityPercent, 0, 100);
        byte alpha = (byte)Math.Round(opacityPercent / 100.0 * 255);
        IntPtr exStyle = NativeMethods.GetWindowLongPtr(window.Handle, NativeMethods.GwlExStyle);
        long layeredStyle = exStyle.ToInt64() | NativeMethods.WsExLayered;
        NativeMethods.SetWindowLongPtr(window.Handle, NativeMethods.GwlExStyle, new IntPtr(layeredStyle));

        if (!NativeMethods.SetLayeredWindowAttributes(window.Handle, 0, alpha, NativeMethods.LwaAlpha))
        {
            throw new Win32Exception("Unable to set opacity for the target window.");
        }
    }
}
