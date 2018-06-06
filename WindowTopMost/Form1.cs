using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowTopMost
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        List<ProcessHnd> WindowList = new List<ProcessHnd>();
        ProcessHnd thisItem = null;

        bool RefreshWindowList()
        {
            WindowList.Clear();
            EnumDelegate filter = delegate (IntPtr hWnd, int lParam)
            {
                StringBuilder strbTitle = new StringBuilder(255);
                int nLength = GetWindowText(hWnd, strbTitle, strbTitle.Capacity + 1);
                string strTitle = strbTitle.ToString();

                if (IsWindowVisible(hWnd) && string.IsNullOrEmpty(strTitle) == false)
                {
                    // Get icon
                    IntPtr hIcon = WinAPI.GetAppIcon(hWnd);

                    Bitmap bitmap = null;
                    if (hIcon != IntPtr.Zero) {
                        bitmap = Icon.FromHandle(hIcon).ToBitmap();
                    }
                    else
                    {
                        bitmap = Icon.FromHandle(WinAPI.LoadIcon(IntPtr.Zero, (IntPtr)WinAPI.SystemIcons.IDI_APPLICATION)).ToBitmap();
                    }

                    // get window topmost info
                    bool isTM = WinAPI.isWindowTopMost(hWnd);

                    // get process info
                    IntPtr hProc = WinAPI.GetProcessHandleFromHwnd(hWnd);
                    
                    StringBuilder fileName = new StringBuilder(2000);
                    string PN = "", Path = "";
                    if (hProc != IntPtr.Zero)
                    {
                        WinAPI.GetProcessImageFileName(hProc, fileName, 2000);
                        PN = fileName.ToString();
                        PN = System.IO.Path.GetFileName(PN);
                        Path = fileName.ToString();
                    }
                    
                    WindowList.Add(new ProcessHnd() { WindowName = strTitle, Handle = hWnd, Icon = bitmap, IsTopMost = isTM, ProcessImagePath = PN, PID = hProc, ProcessFullPath = Path });
                }
                return true;
            };

            return EnumDesktopWindows(IntPtr.Zero, filter, IntPtr.Zero);
        }

        void UpdateProcessList()
        {
            if (!RefreshWindowList())
            {
                MessageBox.Show("Get window list failed");
                return;
            }
            lstWindow.Items.Clear();
            foreach (ProcessHnd H in WindowList)
            {
                lstWindow.Items.Add(H);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            UpdateProcessList();
            btnCancelTopmost.Enabled = btnTopMost.Enabled = false;
        }

        /// <summary>
        /// filter function
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public delegate bool EnumDelegate(IntPtr hWnd, int lParam);

        /// <summary>
        /// check if windows visible
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        /// <summary>
        /// return windows text
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpWindowText"></param>
        /// <param name="nMaxCount"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "GetWindowText",
        ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);

        /// <summary>
        /// enumarator on all desktop windows
        /// </summary>
        /// <param name="hDesktop"></param>
        /// <param name="lpEnumCallbackFunction"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "EnumDesktopWindows",
        ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDelegate lpEnumCallbackFunction, IntPtr lParam);

        /// <summary>
        ///     Retrieves a handle to the top-level window whose class name and window name match the specified strings. This
        ///     function does not search child windows. This function does not perform a case-sensitive search. To search child
        ///     windows, beginning with a specified child window, use the
        ///     <see cref="!:https://msdn.microsoft.com/en-us/library/windows/desktop/ms633500%28v=vs.85%29.aspx">FindWindowEx</see>
        ///     function.
        ///     <para>
        ///     Go to https://msdn.microsoft.com/en-us/library/windows/desktop/ms633499%28v=vs.85%29.aspx for FindWindow
        ///     information or https://msdn.microsoft.com/en-us/library/windows/desktop/ms633500%28v=vs.85%29.aspx for
        ///     FindWindowEx
        ///     </para>
        /// </summary>
        /// <param name="lpClassName">
        ///     C++ ( lpClassName [in, optional]. Type: LPCTSTR )<br />The class name or a class atom created by a previous call to
        ///     the RegisterClass or RegisterClassEx function. The atom must be in the low-order word of lpClassName; the
        ///     high-order word must be zero.
        ///     <para>
        ///     If lpClassName points to a string, it specifies the window class name. The class name can be any name
        ///     registered with RegisterClass or RegisterClassEx, or any of the predefined control-class names.
        ///     </para>
        ///     <para>If lpClassName is NULL, it finds any window whose title matches the lpWindowName parameter.</para>
        /// </param>
        /// <param name="lpWindowName">
        ///     C++ ( lpWindowName [in, optional]. Type: LPCTSTR )<br />The window name (the window's
        ///     title). If this parameter is NULL, all window names match.
        /// </param>
        /// <returns>
        ///     C++ ( Type: HWND )<br />If the function succeeds, the return value is a handle to the window that has the
        ///     specified class name and window name. If the function fails, the return value is NULL.
        ///     <para>To get extended error information, call GetLastError.</para>
        /// </returns>
        /// <remarks>
        ///     If the lpWindowName parameter is not NULL, FindWindow calls the <see cref="M:GetWindowText" /> function to
        ///     retrieve the window name for comparison. For a description of a potential problem that can arise, see the Remarks
        ///     for <see cref="M:GetWindowText" />.
        /// </remarks>
        // For Windows Mobile, replace user32.dll with coredll.dll
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // Find window by Caption only. Note you must pass IntPtr.Zero as the first parameter.

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        private void btnGetHandle_Click(object sender, EventArgs e)
        {
            int S = lstWindow.SelectedIndex;
            if (S == -1)
                return;
            MessageBox.Show(WindowList[S].ToString());
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            UpdateProcessList();
        }

        public const int SWP_ASYNCWINDOWPOS = 0x4000;
        public const int SWP_DEFERERASE = 0x2000;
        public const int SWP_DRAWFRAME = 0x0020;
        public const int SWP_FRAMECHANGED = 0x0020;
        public const int SWP_HIDEWINDOW = 0x0080;
        public const int SWP_NOACTIVATE = 0x0010;
        public const int SWP_NOCOPYBITS = 0x0100;
        public const int SWP_NOMOVE = 0x0002;
        public const int SWP_NOOWNERZORDER = 0x0200;
        public const int SWP_NOREDRAW = 0x0008;
        public const int SWP_NOREPOSITION = 0x0200;
        public const int SWP_NOSENDCHANGING = 0x0400;
        public const int SWP_NOSIZE = 0x0001;
        public const int SWP_NOZORDER = 0x0004;
        public const int SWP_SHOWWINDOW = 0x0040;

        public const int HWND_TOP = 0;
        public const int HWND_BOTTOM = 1;
        public const int HWND_TOPMOST = -1;
        public const int HWND_NOTOPMOST = -2;

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);
        private struct WINDOWPLACEMENT
        {
            public int length;
            public uint flags;
            public int showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        /// <summary>
        /// Sets the show state and the restored, minimized, and maximized positions of the specified window.
        /// </summary>
        /// <param name="hWnd">
        /// A handle to the window.
        /// </param>
        /// <param name="lpwndpl">
        /// A pointer to a WINDOWPLACEMENT structure that specifies the new show state and window positions.
        /// <para>
        /// Before calling SetWindowPlacement, set the length member of the WINDOWPLACEMENT structure to sizeof(WINDOWPLACEMENT). SetWindowPlacement fails if the length member is not set correctly.
        /// </para>
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// <para>
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
        /// </para>
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWindowPlacement(IntPtr hWnd,
           [In] ref WINDOWPLACEMENT lpwndpl);

        void setTopmostState(int hnd)
        {
            int S = lstWindow.SelectedIndex;
            if (S == -1)
                return;
            RECT R = new RECT();
            WINDOWPLACEMENT P = new WINDOWPLACEMENT();
            bool success = GetWindowRect(thisItem.Handle, out R) &&
                GetWindowPlacement(thisItem.Handle, ref P);
            if (!success)
            {
                MessageBox.Show("无法获取窗口信息。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!SetWindowPos(thisItem.Handle, (IntPtr)hnd, R.Left, R.Top, R.Right - R.Left, R.Bottom - R.Top, P.flags))
            {
                MessageBox.Show("设置窗口位置时发生错误。\r\n如果该程序是以管理员权限运行的，那么你需要以管理员权限运行本程序。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            updateTopMostInfo(thisItem.Handle);
            thisItem.IsTopMost = lstWindow.Items[S].IsTopMost = WinAPI.isWindowTopMost(thisItem.Handle);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            thisItem = WindowList[lstWindow.SelectedIndex];
            setTopmostState(HWND_TOPMOST);
        }

        private void btnCancelTopmost_Click(object sender, EventArgs e)
        {
            thisItem = WindowList[lstWindow.SelectedIndex];
            setTopmostState(HWND_NOTOPMOST);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics G = e.Graphics;
            G.DrawLine(new Pen(Color.LightGray), 0, 0, panel1.Width, 0);
        }

        private void button_selected_Click(object sender, EventArgs e)
        {
            MessageBox.Show(lstWindow.SelectedIndex.ToString());
        }

        void updateTopMostInfo(IntPtr hWnd)
        {
            bool isTopMost = WinAPI.isWindowTopMost(hWnd);
            btnCancelTopmost.Enabled = isTopMost;
            btnTopMost.Enabled = !isTopMost;
        }

        private void lstWindow_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(lstWindow.SelectedIndex.ToString());
            btnCancelTopmost.Enabled = btnTopMost.Enabled = lstWindow.SelectedIndex != -1;

            if (lstWindow.SelectedIndex != -1)
            {
                ProcessHnd H = WindowList[lstWindow.SelectedIndex];
                updateTopMostInfo(H.Handle);
            }
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            Graphics G = e.Graphics;
            G.DrawLine(new Pen(Color.LightGray), 0, 0, panel3.Width, 0);
        }

        private void lstWindow_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void lstWindow_MouseUp(object sender, MouseEventArgs e)
        {
            MiyukiListBox lst = (MiyukiListBox)lstWindow;

            if (lst.HoverIndex == -1)
                return;

            lst.SelectedIndex = lst.HoverIndex;

            if (e.Button == MouseButtons.Right)
            {
                thisItem = WindowList[lst.HoverIndex];
                menuWindowInfo.Text = "Handle ID: " + thisItem.Handle.ToString();
                menuProcessName.Text = thisItem.ProcessImagePath != "" ? "Process: " + thisItem.ProcessImagePath : "No process information";

                if (this.thisItem.Handle == IntPtr.Zero)
                {
                    menuTopMost.Enabled = menuUntopMost.Enabled = false;
                }
                else
                {
                    bool isTopMost = WinAPI.isWindowTopMost(this.thisItem.Handle);
                    menuTopMost.Enabled = !isTopMost;
                    menuUntopMost.Enabled = isTopMost;
                }

                menuOpenProcLocation.Enabled = thisItem.ProcessImagePath != "";

                rightClickMenu.Show(this, e.Location);
            }
        }

        private void menuTopMost_Click(object sender, EventArgs e)
        {
            setTopmostState(HWND_TOPMOST);
        }

        private void menuUntopMost_Click(object sender, EventArgs e)
        {
            setTopmostState(HWND_NOTOPMOST);
        }

        private Dictionary<string, string> Drives = null;

        private Dictionary<string, string> getDriveInfo()
        {
            Dictionary<string, string> Drives = new Dictionary<string, string>();
            DriveInfo[] Ds = DriveInfo.GetDrives();
            foreach (DriveInfo D in Ds)
            {
                string name = D.Name.Replace("\\", "");
                StringBuilder SB = new StringBuilder(2000);
                WinAPI.QueryDosDevice(name, SB, 2000);
                Drives[SB.ToString()] = name;
            }
            return Drives;
        }

        private void menuOpenProcLocation_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(drivers[0].);
            if (thisItem.ProcessFullPath != "")
            {
                if (Drives == null)
                    Drives = getDriveInfo();
                Regex reg = new Regex(@"(\\Device\\[\w\d]+)\\");
                Match M = reg.Match(thisItem.ProcessFullPath);
                if (!M.Success)
                {
                    MessageBox.Show("unknown location: " + thisItem.ProcessFullPath);
                    return;
                }
                string capture = M.Groups[1].Value;
                try
                {
                    string letter = Drives[capture];
                    string location = thisItem.ProcessFullPath.Replace(capture, letter);
                    
                    Process.Start("explorer.exe", "/select,\""+location+"\"");
                }
                catch(Exception ee)
                {
                    MessageBox.Show(ee.Message);
                }
            }
        }
    }
}
