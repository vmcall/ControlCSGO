using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control
{
    public static class NumericExtensions
    {
        public static string Hex(this int integer) => $"0x{integer.ToString("X")}";
        public static string Hex(this IntPtr ptrInteger) => $"0x{ptrInteger.ToString("X")}";

        public static double ToRadians(this double val) => (Math.PI / 180) * val;
    }
}
