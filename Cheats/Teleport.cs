using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Control.Cheats
{
    public static class Teleport
    {
        public static void Activate()
        {
            uint dwBack = 0;
            Windows.VirtualProtect(Offsets.TeleportOffset, 4, (uint)Protection.PAGE_READWRITE, out dwBack);
            byte[] bBytes = { 0xBE, 0x1 };
            M.Write(bBytes, Offsets.TeleportOffset);
            Windows.VirtualProtect(Offsets.TeleportOffset, 4, dwBack, out dwBack);

            Thread.Sleep(50);

            dwBack = 0;
            Windows.VirtualProtect(Offsets.TeleportOffset, 4, (uint)Protection.PAGE_READWRITE, out dwBack);
            byte[] bBytes2 = { 0xBE, 0x20 };
            M.Write(bBytes2, Offsets.TeleportOffset);
            Windows.VirtualProtect(Offsets.TeleportOffset, 4, dwBack, out dwBack);
        }
    }
}
