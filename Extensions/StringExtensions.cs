using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control
{
    public static class StringExtensions
    {
        public static bool DigitsOnly(this string str) => str.All(c => c >= '0' && c <= '9');
    }
}
