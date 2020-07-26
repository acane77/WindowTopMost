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
            WinAPI.EnumDelegate filter = delegate (IntPtr hWnd, int lParam)
            {
                StringBuilder strbTitle = new StringBuilder(255);
                int nLength = WinAPI.GetWindowText(hWnd, strbTitle, strbTitle.Capacity + 1);
                string strTitle = strbTitle.ToString();

                if (WinAPI.IsWindowVisible(hWnd) && string.IsNullOrEmpty(strTitle) == false)
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

                    // get executable description
                    string logicalPath = getLogicalFilePath(Path);
                    string description = null;
                    try {
                        if (logicalPath == null)
                            throw new Exception();
                        FileVersionInfo F = System.Diagnostics.FileVersionInfo.GetVersionInfo(logicalPath);
                        description = F.FileDescription;
                    }
                    catch (Exception ee)
                    {
                        description = "";
                    }

                    // try get application icon
                    if (hIcon == IntPtr.Zero)
                    {
                        hIcon = extractIconFromFile(logicalPath);
                        if (hIcon != IntPtr.Zero)
                            bitmap = Icon.FromHandle(hIcon).ToBitmap();
                    }

                    // Get window opacity
                    bool canSetOpacity = true;
                    byte transparency = GetWindowTransparency(hWnd, out canSetOpacity);

                    WindowList.Add(new ProcessHnd() { WindowName = strTitle, Handle = hWnd, Icon = bitmap, IsTopMost = isTM, ProcessImagePath = PN, PID = hProc, 
                        ProcessFullPath = Path, Description = description, LogicalPath = logicalPath, WindowOpacity = transparency, CanSetWindowOpacity = canSetOpacity });
                }
                return true;
            };

            return WinAPI.EnumDesktopWindows(IntPtr.Zero, filter, IntPtr.Zero);
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

            // Register events
            foreach (MenuItem menu in menuItemSetOpacity.MenuItems)
            {
                string menuOpacityValue = menu.Text.Split("%".ToCharArray())[0];
                try
                {
                    byte opacityValue = (byte)(Int32.Parse(menuOpacityValue) / 100.0 * 255);
                    menu.Click += (object sender_, EventArgs e_) =>
                    {
                        SetWindowTransparency(thisItem.Handle, opacityValue);
                    };
                }
                catch
                {
                    menu.Click += (object sender_, EventArgs e_) =>
                    {
                        FormSetOpacity frm = new FormSetOpacity();
                        frm.SetInitlalValue(thisItem.WindowOpacity);
                        frm.ShowDialog();
                        if (frm.TransparenctChanged)
                        {
                            SetWindowTransparency(thisItem.Handle, (byte)(frm.TransparencyValue / 100.0 * 255));
                        }
                    };   
                }
            }
        }

        void setTopmostState(int hnd)
        {
            int S = lstWindow.SelectedIndex;
            if (S == -1)
                return;
            WinAPI.RECT R = new WinAPI.RECT();
            WinAPI.WINDOWPLACEMENT P = new WinAPI.WINDOWPLACEMENT();
            bool success = WinAPI.GetWindowRect(thisItem.Handle, out R) &&
                WinAPI.GetWindowPlacement(thisItem.Handle, ref P);
            if (!success)
            {
                MessageBox.Show("无法获取窗口信息。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!WinAPI.SetWindowPos(thisItem.Handle, (IntPtr)hnd, R.Left, R.Top, R.Right - R.Left, R.Bottom - R.Top, P.flags))
            {
                MessageBox.Show("设置窗口位置时发生错误。\r\n如果该程序是以管理员权限运行的，那么你需要以管理员权限运行本程序。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            updateTopMostInfo(thisItem.Handle);
            thisItem.IsTopMost = lstWindow.Items[S].IsTopMost = WinAPI.isWindowTopMost(thisItem.Handle);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            thisItem = WindowList[lstWindow.SelectedIndex];
            setTopmostState(WinAPI.HWND_TOPMOST);
        }

        private void btnCancelTopmost_Click(object sender, EventArgs e)
        {
            thisItem = WindowList[lstWindow.SelectedIndex];
            setTopmostState(WinAPI.HWND_NOTOPMOST);
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
                
                menuItemProcessName.Text = "Handle: " + thisItem.Handle.ToString();
                menuItemWindowName.Text = thisItem.ProcessImagePath != "" ? "Process: " + thisItem.ProcessImagePath : "No process information";

                if (this.thisItem.Handle == IntPtr.Zero)
                {
                    menuItemWindowTopMost.Enabled = menuItemCancelTopmost.Enabled = false;
                }
                else
                {
                    bool isTopMost = WinAPI.isWindowTopMost(this.thisItem.Handle);
                    menuItemWindowTopMost.Enabled = !isTopMost;
                    menuItemCancelTopmost.Enabled = isTopMost;
                }

                menuItemOpenLocation.Enabled = thisItem.ProcessImagePath != "";

                // set checked state with current opacity
                byte opacity = thisItem.WindowOpacity;
                menuItemSetOpacityTo100.Checked = thisItem.CanSetWindowOpacity && (opacity == 255);
                menuItemSetOpacityTo90.Checked = thisItem.CanSetWindowOpacity && (opacity == 229);
                menuItemSetOpacityTo75.Checked = thisItem.CanSetWindowOpacity && (opacity == 191);
                menuItemSetOpacityTo50.Checked = thisItem.CanSetWindowOpacity && (opacity == 127);
                menuItemSetOpacityTo25.Checked = thisItem.CanSetWindowOpacity && (opacity == 63);
                menuItemSetOpacityTo0.Checked = thisItem.CanSetWindowOpacity && (opacity == 0);
                //menuItemSetOpacity.Enabled = thisItem.CanSetWindowOpacity;

                contextMenu.Show(this, e.Location);
            }
        }

        private void menuTopMost_Click(object sender, EventArgs e)
        {
            setTopmostState(WinAPI.HWND_TOPMOST);
        }

        private void menuUntopMost_Click(object sender, EventArgs e)
        {
            setTopmostState(WinAPI.HWND_NOTOPMOST);
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

        string getLogicalFilePath(string physical)
        {
            if (Drives == null)
                Drives = getDriveInfo();
            Regex reg = new Regex(@"(\\Device\\[\w\d]+)\\");
            Match M = reg.Match(physical);
            if (!M.Success)
            {
                //MessageBox.Show("unknown location: " + physical);
                return null;
            }
            string capture = M.Groups[1].Value;
            try
            {
                string letter = Drives[capture];
                string location = physical.Replace(capture, letter);

                return location;
            }
            catch (Exception ee)
            {
                //MessageBox.Show(ee.Message);
                return null;
            }
        }

        private void menuOpenProcLocation_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(drivers[0].);
            if (thisItem.ProcessFullPath != "")
            {
                string location = thisItem.LogicalPath;
                if (location != null)
                    Process.Start("explorer.exe", "/select,\"" + location + "\"");
            }
        }

        private void menuSwitchTo_Click(object sender, EventArgs e)
        {
            if (thisItem.Handle != IntPtr.Zero)
            {
                WinAPI.WINDOWPLACEMENT P = new WinAPI.WINDOWPLACEMENT();
                WinAPI.RECT R = new WinAPI.RECT();
                bool success = WinAPI.GetWindowRect(thisItem.Handle, out R) &&
                WinAPI.GetWindowPlacement(thisItem.Handle, ref P);
                if (!success)
                {
                    MessageBox.Show("无法获取窗口信息。\r\n\r\n可能是该窗口已经关闭。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                WinAPI.SwitchToThisWindow(thisItem.Handle, true);
            }
        }

        IntPtr extractIconFromFile(string filename)
        {
            IntPtr[] L = new IntPtr[1] { IntPtr.Zero };
            IntPtr[] S = new IntPtr[1] { IntPtr.Zero };
            WinAPI.ExtractIconEx(filename, 0, L, S, 1);
            return L[0] == IntPtr.Zero ? S[0] : L[0];
        }

        private void lstWindow_DoubleClick(object sender, EventArgs e)
        {
            thisItem = WindowList[lstWindow.SelectedIndex];
            if (thisItem.Handle != IntPtr.Zero)
            {
                WinAPI.WINDOWPLACEMENT P = new WinAPI.WINDOWPLACEMENT();
                WinAPI.RECT R = new WinAPI.RECT();
                bool success = WinAPI.GetWindowRect(thisItem.Handle, out R) &&
                WinAPI.GetWindowPlacement(thisItem.Handle, ref P);
                if (!success)
                {
                    MessageBox.Show("无法获取窗口信息。\r\n\r\n可能是该窗口已经关闭。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                WinAPI.SwitchToThisWindow(thisItem.Handle, true);
            }
        }

        private void menuItemSwitchTo_Click(object sender, EventArgs e)
        {
            menuSwitchTo_Click(sender, e);
        }

        private void menuItemWindowTopMost_Click(object sender, EventArgs e)
        {
            menuTopMost_Click(sender, e);
        }

        private void menuItemCancelTopmost_Click(object sender, EventArgs e)
        {
            menuUntopMost_Click(sender, e);
        }

        private void menuItemOpenLocation_Click(object sender, EventArgs e)
        {
            menuOpenProcLocation_Click(sender, e);
        }

        private void menuGetInfo_Click(object sender, EventArgs e)
        {
            MiyukiListBox lst = (MiyukiListBox)lstWindow;

            if (lst.HoverIndex == -1)
                return;

            thisItem = WindowList[lst.HoverIndex];

            string info = "窗口句柄: " + thisItem.Handle.ToString() + "\r\n";
            info += "窗口名称：" + thisItem.WindowName + "\r\n";
            info += thisItem.ProcessImagePath != "" ? "进程名: " + thisItem.ProcessImagePath : "无法获取到进程信息，请使用管理员权限运行本程序后再试。";
            info += "\r\n";
            info += "进程逻辑位置：" + thisItem.LogicalPath + "\r\n";
            info += "进程物理位置：" + thisItem.ProcessFullPath + "\r\n";

            MessageBox.Show(info, !String.IsNullOrEmpty(thisItem.WindowName) ? thisItem.WindowName : thisItem.Description, MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        public void SetWindowTransparency(IntPtr WindowHandle, byte Alpha, ProcessHnd p = null)
        {
            IntPtr windowInfoPtr = WinAPI.GetWindowLongPtr(WindowHandle, WinAPI.GWL_EXSTYLE);
            int WindowLong = windowInfoPtr.ToInt32() | WinAPI.WS_EX_LAYERED;
            WinAPI.SetWindowLongPtr(WindowHandle, WinAPI.GWL_EXSTYLE, IntPtr.Add(IntPtr.Zero, WindowLong));
            bool success = WinAPI.SetLayeredWindowAttributes(WindowHandle, 0, Alpha, WinAPI.LWA_ALPHA);
            if (!success)
            {
                MessageBox.Show("无法设置该窗口的透明度", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (p == null)
                p = thisItem;

            bool canSetTransparency = true;
            byte newAlpha = GetWindowTransparency(WindowHandle, out canSetTransparency);
            p.CanSetWindowOpacity = canSetTransparency;
            p.WindowOpacity = newAlpha;
            if (!canSetTransparency)
            {
                MessageBox.Show("无法设置该窗口的透明度", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public byte GetWindowTransparency(IntPtr WindowHandle, out bool canSetTransparency)
        {
            uint ckey = 0;
            byte alpha = 0;
            uint lwa = 0;
            WinAPI.GetLayeredWindowAttributes(WindowHandle, out ckey, out alpha, out lwa);
            IntPtr windowInfoPtr = WinAPI.GetWindowLongPtr(WindowHandle, WinAPI.GWL_EXSTYLE);
            canSetTransparency = (windowInfoPtr.ToInt32() & WinAPI.WS_EX_LAYERED) > 0 && lwa > 0;
            return alpha;
        }


    }
}
