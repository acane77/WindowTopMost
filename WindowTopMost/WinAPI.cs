using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowTopMost
{
    public class WinAPI
    {
        public static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size > 4)
                return GetClassLongPtr64(hWnd, nIndex);
            else
                return GetClassLongPtr32(hWnd, nIndex);
        }


        [DllImport("user32.dll", EntryPoint = "GetClassLong")]
        public static extern IntPtr GetClassLongPtr32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
        public static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);


        public enum ClassLongFlags : int
        {
            GCLP_MENUNAME = -8,
            GCLP_HBRBACKGROUND = -10,
            GCLP_HCURSOR = -12,
            GCLP_HICON = -14,
            GCLP_HMODULE = -16,
            GCL_CBWNDEXTRA = -18,
            GCL_CBCLSEXTRA = -20,
            GCLP_WNDPROC = -24,
            GCL_STYLE = -26,
            GCLP_HICONSM = -34,
            GCW_ATOM = -32
        }

        [DllImport("user32.dll")]
        public static extern IntPtr LoadIcon(IntPtr hInstance, string lpIconName);
        [DllImport("user32.dll")]
        public static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIconName);


        public enum SystemIcons
        {
            IDI_APPLICATION = 32512,
            IDI_HAND = 32513,
            IDI_QUESTION = 32514,
            IDI_EXCLAMATION = 32515,
            IDI_ASTERISK = 32516,
            IDI_WINLOGO = 32517,
            IDI_WARNING = IDI_EXCLAMATION,
            IDI_ERROR = IDI_HAND,
            IDI_INFORMATION = IDI_ASTERISK,
        }

        [DllImport("user32.dll", EntryPoint = "GetClassLong")]
        public static extern uint GetClassLongPtr32(HandleRef hWnd, int nIndex);

    }
}
