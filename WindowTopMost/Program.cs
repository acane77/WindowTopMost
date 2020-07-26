using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowTopMost
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 检查操作系统版本
            string Version = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows NT\CurrentVersion", "ProductName", null);
            if (Version.IndexOf("10") <= 0)
            {
                MessageBox.Show("此程序仅可以在Windows 10下运行。", "Window Top-Mostify", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //return;
            }
            frmMain MainForm = new frmMain();
            Application.Run(MainForm);
        }
    }
}
