using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowTopMost
{
    public class IconInfo : IDisposable
    {
        private ICONINFO ii;

        public IconInfo(IntPtr hCursor)
        {
            if (!GetIconInfo(hCursor, out ii))
            {
                throw new Exception("Bad hCursor");
            }
        }

        public Bitmap ColorBitmap
        {
            get
            {
                if (ii.hbmColor == IntPtr.Zero) return null;
                return Image.FromHbitmap(ii.hbmColor);
            }
        }

        public Bitmap MaskBitmap
        {
            get
            {
                if (ii.hbmMask == IntPtr.Zero) return null;
                return Image.FromHbitmap(ii.hbmMask);
            }
        }
        public Point HotPoint
        {
            get
            {
                return new Point(ii.xHotspot, ii.yHotspot);
            }
        }

        void IDisposable.Dispose()
        {
            if (ii.hbmColor != IntPtr.Zero) DeleteObject(ii.hbmColor);
            if (ii.hbmMask != IntPtr.Zero) DeleteObject(ii.hbmMask);
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        static extern IntPtr DeleteObject(IntPtr hDc);

        [DllImport("user32.dll")]
        static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);

        [StructLayout(LayoutKind.Sequential)]
        struct ICONINFO
        {
            public bool fIcon;
            public Int32 xHotspot;
            public Int32 yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        //
        // This functon used to mask(multiply) two images bitmap.
        //
        public static Bitmap MaskImagePtr(Bitmap SrcBitmap1, Bitmap SrcBitmap2, out string Message)
        {
            int width;
            int height;

            Message = "";

            if (SrcBitmap1.Width < SrcBitmap2.Width)
                width = SrcBitmap1.Width;
            else
                width = SrcBitmap2.Width;

            if (SrcBitmap1.Height < SrcBitmap2.Height)
                height = SrcBitmap1.Height;
            else
                height = SrcBitmap2.Height;

            var bitmap = new Bitmap(width, height);

            try
            {
                BitmapData S1D = SrcBitmap1.LockBits(new Rectangle(0, 0, SrcBitmap1.Width, SrcBitmap1.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                BitmapData S2D = SrcBitmap2.LockBits(new Rectangle(0, 0, SrcBitmap2.Width, SrcBitmap2.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                BitmapData DD = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                unsafe
                {
                    int X = 3, DX = 4;

                    for (int col = 0; col < bitmap.Height; col++)
                    {
                        byte* S1 = (byte*)S1D.Scan0 + col * S1D.Stride;
                        byte* S2 = (byte*)S2D.Scan0 + col * S2D.Stride;
                        byte* D = (byte*)DD.Scan0 + col * DD.Stride;

                        for (int row = 0; row < bitmap.Width; row++)
                        {
                            D[row * DX] = (byte)(S2[row * X] & ~S1[row * X]);
                            D[row * DX + 1] = (byte)(S2[row * X + 1] & ~S1[row * X + 1]);
                            D[row * DX + 2] = (byte)(S2[row * X + 2] & ~S1[row * X + 2]);
                            D[row * DX + 3] = 255;

                            if (D[row * DX] == 0 && D[row * DX + 1] == 0 && D[row * DX + 2] == 0)
                            {
                                D[row * DX] = D[row * DX + 1] = D[row * DX + 2] = 255;
                                D[row * DX + 3] = 0;
                            }
                        }
                    }
                }

                bitmap.UnlockBits(DD);
                SrcBitmap1.UnlockBits(S1D);
                SrcBitmap2.UnlockBits(S2D);

                SrcBitmap1.Dispose();
                SrcBitmap2.Dispose();
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }

            return bitmap;
        }
    }
}
