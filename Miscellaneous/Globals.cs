using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control
{
    class G
    {
        public static List<CSPlayer> EntityList { get; set; } = new List<CSPlayer>();
        public static List<CSPlayer> TargetList { get; set; } = new List<CSPlayer>();

        public static IntPtr ClientBase
        {
            get
            {
                return ClientModule.BaseAddress;
            }
        }
        public static IntPtr EngineBase
        {
            get
            {
                return EngineModule.BaseAddress;
            }
        }
        public static IntPtr MaterialSystemBase
        {
            get
            {
                return MaterialSystemModule.BaseAddress;
            }
        }
        public static IntPtr GlowPointer { get; set; }

        public static ProcessModule ClientModule { get; set; }
        public static ProcessModule EngineModule { get; set; }
        public static ProcessModule MaterialSystemModule { get; set; }
        public static ProcessModule Tier0Module { get; set; }


        public static CSPlayer LocalPlayer { get; set; }
        public static Matrix ViewMatrix { get; set; }
        public static Process GlobalOffensive;

        public static IntPtr ClientState() => M.Read<IntPtr>(G.EngineBase + Offsets.m_dwClientState);

        public static readonly object lockObj = new object();
    }
}
