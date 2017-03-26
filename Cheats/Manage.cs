using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Control.Cheats
{
    public static class Manage
    {
        private static Thread BunnyhopThread    = new Thread(Bunnyhop.Run);
        private static Thread EspThread         = new Thread(Visuals.Run);
        private static Thread AimbotThread      = new Thread(Aimbot.Run);
        private static Thread NoFlashThread     = new Thread(Misc.NoFlash);
        private static Thread LagThread         = new Thread(Misc.Lag);
        private static Thread SkinChangerThread = new Thread(Skinchanger.Run);
        private static Thread TriggerThread     = new Thread(Triggerbot.Run);
        private static Thread BypassThread      = new Thread(Bypass.Run);

        #region Toggles
        public static void ToggleNoFlash (bool NoFlashEnabled)
        {
            if (NoFlashEnabled)
                NoFlashThread.Start();
            else
            {
                NoFlashThread.Abort();
                NoFlashThread = new Thread(Misc.NoFlash);
            }
        }
        public static void ToggleBunnyhop (bool BhopEnabled)
        {
            if (BhopEnabled)
                BunnyhopThread.Start();
            else
            {
                BunnyhopThread.Abort();
                BunnyhopThread = new Thread(Bunnyhop.Run);
            }
            
        }
        public static void ToggleEsp (bool EspEnabled)
        {
            if (EspEnabled)
                EspThread.Start();
            else
            {
                EspThread.Abort();
                EspThread = new Thread(Visuals.Run);
            }
        }
        public static void ToggleAimbot (bool AimbotEnabled)
        {
            if (AimbotEnabled)
                AimbotThread.Start();
            else
            {
                AimbotThread.Abort();
                AimbotThread = new Thread(Aimbot.Run);
            }
        }
        public static void ToggleLag (bool LagEnabled)
        {
            if (LagEnabled)
                LagThread.Start();
            else
            {
                LagThread.Abort();
                LagThread = new Thread(Misc.Lag);
                H.SendPacket = true;
            }
        }
        public static void ToggleSkinChanger (bool SkinChangerEnabled)
        {
            if (SkinChangerEnabled)
                SkinChangerThread.Start();
            else
            {
                SkinChangerThread.Abort();
                SkinChangerThread = new Thread(Skinchanger.Run);
            }
        }
        public static void ToggleTriggerbot(bool TriggerEnabled)
        {
            if (TriggerEnabled)
                TriggerThread.Start();
            else
            {
                TriggerThread.Abort();
                TriggerThread = new Thread(Triggerbot.Run);
            }
        }
        public static void ToggleBypass(bool BypassEnabled)
        {
            if (BypassEnabled)
                BypassThread.Start();
            else
            {
                BypassThread.Abort();
                BypassThread = new Thread(Bypass.Run);
                H.ResetConvars();
            }
        }
        #endregion

        public static void UpdateThread()
        {
            var tempEntList = new List<CSPlayer>();
            var tempTargetList = new List<CSPlayer>();

            new Thread(() =>
            {
                while (true)
                {
                    if (G.GlobalOffensive.HasExited)
                        H.SafeExit();

                    G.LocalPlayer = new CSPlayer(G.ClientBase + Offsets.m_dwLocalPlayer);

                    tempEntList.Clear();
                    tempTargetList.Clear();

                    for (int x = 1; x <= 64; x++)
                    {
                        CSPlayer ent = H.GetPlayer(x);

                        if (!ent.Valid)
                            continue;

                        tempEntList.Add(ent);

                        if (!ent.Teammate)
                            tempTargetList.Add(ent);
                    }

                    G.TargetList = tempTargetList;
                    G.EntityList = tempEntList;

                    Thread.Sleep(100);
                }

            }).Start();
        }
    }
}
