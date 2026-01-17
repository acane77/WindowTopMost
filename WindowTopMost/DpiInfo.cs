using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowTopMost
{
    internal class DpiInfo
    {
        public static float scale = 1.0f;

        public static float GetScale(IntPtr window_ptr, bool make_public = true)
        {
            if (window_ptr == IntPtr.Zero)
                return scale; // 返回当前scale值，避免无效Handle
            
            using (Graphics graphics = Graphics.FromHwnd(window_ptr))
            {
                float new_scale = graphics.DpiX / 96f;
                if (make_public)
                    scale = new_scale;
                return new_scale;
            }
        }
    }
}
