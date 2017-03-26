using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace Control
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CheatForm());
        }
    }
}
