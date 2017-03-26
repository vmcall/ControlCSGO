using Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Control.Cheats
{
    public static class Misc
    {
        public static void NoFlash()
        {
            while (true)
            {
                if (G.LocalPlayer.FlashDuration > 0)
                    G.LocalPlayer.FlashDuration = 0;

                Thread.Sleep(1);
            }
        }

        public static int LagAmount = 0;
        public static void Lag()
        {
            int lag = -1;

            while (true)
            {
                lag++;

                if (lag < LagAmount)
                    H.SendPacket = false;
                else
                {
                    H.SendPacket = true;
                    lag = 0;
                }

                Thread.Sleep(1);
            }
        }
    }
}
