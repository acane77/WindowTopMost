using System.Runtime.InteropServices;
using System.Text;

namespace MelinaWindowTopmost.Native;

internal static class NativeMethods
{
    public const int GwlExStyle = -20;
    public const int WsExTopMost = 0x00000008;
    public const int WsExLayered = 0x00080000;
    public const uint LwaAlpha = 0x00000002;

    public static readonly IntPtr HwndTopMost = new(-1);
    public static readonly IntPtr HwndNoTopMost = new(-2);

    public const uint SwpNoSize = 0x0001;
    public const uint SwpNoMove = 0x0002;
    public const uint SwpNoActivate = 0x0010;
    public const uint SwpShowWindow = 0x0040;

    public const int IconSmall = 0;
    public const int IconBig = 1;
    public const int IconSmall2 = 2;
    public const int WmGetIcon = 0x007F;
    public const int GclpHIcon = -14;
    public const int GclpHIconSmall = -34;

    private const uint ShgfiIcon = 0x000000100;
    private const uint ShgfiLargeIcon = 0x000000000;
    private const uint ShgfiUseFileAttributes = 0x000000010;
    private const uint FileAttributeNormal = 0x00000080;
    private const int IdiApplication = 32512;

    public const uint ProcessQueryLimitedInformation = 0x1000;

    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumWindowsProc callback, IntPtr lParam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int maxCount);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int processId);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", SetLastError = true)]
    private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "GetWindowLong", SetLastError = true)]
    private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

    public static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
    {
        return IntPtr.Size == 8 ? GetWindowLongPtr64(hWnd, nIndex) : GetWindowLongPtr32(hWnd, nIndex);
    }

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
    private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr value);

    [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
    private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int value);

    public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr value)
    {
        return IntPtr.Size == 8
            ? SetWindowLongPtr64(hWnd, nIndex, value)
            : new IntPtr(SetWindowLong32(hWnd, nIndex, value.ToInt32()));
    }

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint flags);

    [DllImport("user32.dll")]
    public static extern void SwitchToThisWindow(IntPtr hWnd, bool altTab);

    [DllImport("user32.dll")]
    public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, uint colorKey, byte alpha, uint flags);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool GetLayeredWindowAttributes(IntPtr hWnd, out uint colorKey, out byte alpha, out uint flags);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessage(IntPtr hWnd, int message, int wParam, int lParam);

    [DllImport("user32.dll", EntryPoint = "GetClassLongPtr", SetLastError = true)]
    private static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "GetClassLong", SetLastError = true)]
    private static extern IntPtr GetClassLongPtr32(IntPtr hWnd, int nIndex);

    public static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
    {
        return IntPtr.Size == 8 ? GetClassLongPtr64(hWnd, nIndex) : GetClassLongPtr32(hWnd, nIndex);
    }

    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    public static extern uint ExtractIconEx(string fileName, int iconIndex, IntPtr[] largeIcons, IntPtr[] smallIcons, uint iconCount);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DestroyIcon(IntPtr icon);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr LoadIcon(IntPtr instance, IntPtr iconName);

    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SHGetFileInfo(string path, uint fileAttributes, ref SHFILEINFO fileInfo, uint fileInfoSize, uint flags);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr OpenProcess(uint desiredAccess, bool inheritHandle, int processId);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CloseHandle(IntPtr handle);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool QueryFullProcessImageName(IntPtr process, int flags, StringBuilder exeName, ref int size);

    public static IntPtr GetAppIcon(IntPtr hWnd)
    {
        IntPtr icon = SendMessage(hWnd, WmGetIcon, IconBig, 0);
        if (icon == IntPtr.Zero)
        {
            icon = GetClassLongPtr(hWnd, GclpHIcon);
        }

        if (icon == IntPtr.Zero)
        {
            icon = SendMessage(hWnd, WmGetIcon, IconSmall, 0);
        }

        if (icon == IntPtr.Zero)
        {
            icon = SendMessage(hWnd, WmGetIcon, IconSmall2, 0);
        }

        if (icon == IntPtr.Zero)
        {
            icon = GetClassLongPtr(hWnd, GclpHIconSmall);
        }

        return icon;
    }

    public static IntPtr GetModernApplicationIcon()
    {
        try
        {
            SHFILEINFO shfi = new();
            IntPtr result = SHGetFileInfo(
                ".exe",
                FileAttributeNormal,
                ref shfi,
                (uint)Marshal.SizeOf(typeof(SHFILEINFO)),
                ShgfiIcon | ShgfiLargeIcon | ShgfiUseFileAttributes);

            if (result != IntPtr.Zero && shfi.hIcon != IntPtr.Zero)
            {
                return shfi.hIcon;
            }
        }
        catch
        {
        }

        return LoadIcon(IntPtr.Zero, new IntPtr(IdiApplication));
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct SHFILEINFO
    {
        public IntPtr hIcon;
        public int iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    }
}
