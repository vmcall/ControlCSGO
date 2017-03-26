using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Control
{
    public static class ProcessExtensions
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public static bool Active(this Process p) => GetForegroundWindow() == p.MainWindowHandle;
    }
}
