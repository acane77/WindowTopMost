using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowTopMost
{
    public class ProcessHnd : IDisposable
    {

        public Guid ID { get; set; }

        public string WindowName { get; set; }

        public IntPtr Handle { get; set; }

        public bool IsFocus { get; set; }

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
