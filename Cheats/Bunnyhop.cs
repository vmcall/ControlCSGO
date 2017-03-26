using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Control.Cheats
{
    public enum BhopMode
    {
        Normal,
        Humanized
    }

    public static class Bunnyhop
    {
        public static BhopMode BunnyhopMode = BhopMode.Normal;
        public static bool AutoStrafe = false;
        public static int MaxJumps = 4;
        private static int JumpsDone = 0;
        private static bool shouldResetMovement = false;

        public static void Run()
        {
            float lastPitch = H.ViewAngles.Y;

            while (true)
            {
                Thread.Sleep(1);

                if (!G.GlobalOffensive.Active())
                    continue;

                if (!H.KeyPressed(Keys.Space))
                {
                    if (shouldResetMovement && AutoStrafe)
                    {
                        lastPitch = H.ViewAngles.Y;
                        shouldResetMovement = false;
                        Controls.Left = false;
                        Controls.Right = false;
                    }
                    continue;
                }

                shouldResetMovement = false;

                if (G.LocalPlayer.MovementFlag(MovementFlag.ON_GROUND))
                {
                    #region Humanized / SMAC
                    if (BunnyhopMode == BhopMode.Humanized)
                    {
                        Thread.Sleep(10 + H.RandomInt(0, 2));

                        JumpsDone++;

                        if (JumpsDone > MaxJumps + H.RandomInt(0, 1))
                        {
                            JumpsDone = 0;
                            Thread.Sleep(H.RandomInt(20, 40));
                        }

                        for (int x = 0; x < 3; x++)
                        {
                            Controls.Jump();
                            Thread.Sleep(50);
                        }

                        continue;
                    }
                    #endregion

                    Controls.Jump();
                }
                else if (AutoStrafe)
                {
                    shouldResetMovement = true;

                    float pitch = H.ViewAngles.Y;

                    if (pitch != lastPitch)
                    {
                        Controls.Right = pitch < lastPitch;
                        Controls.Left = pitch > lastPitch;
                    }

                    lastPitch = pitch;
                }
            }
        }
    }
}
