using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowTopMost
{
    public class ProcessHnd : IDisposable
    {
        
        public Bitmap Icon { get; set; }

        public Guid ID { get; set; }

        public string WindowName { get; set; }

        public IntPtr Handle { get; set; }

        public bool IsFocus { get; set; }

        public bool IsTopMost { get; set; }

        public string ProcessImagePath { get; set; }

        public IntPtr PID { get; set; }

        public string ProcessFullPath { get; set; }

        public string Description { get; set; }

        public string LogicalPath { get; set; }

        public byte WindowOpacity { get; set; }

        public bool CanSetWindowOpacity { get; set; }

        public ProcessHnd(Guid iD, string windowName, IntPtr handle)
        {
            ID = iD;
            WindowName = windowName;
            Handle = handle;
        }

        public ProcessHnd()
        {
        }

        public void Dispose()
        {
            
        }
    }
}
