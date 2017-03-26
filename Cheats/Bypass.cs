using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Control.Cheats
{
    // ALL CREDITS GO TO Casual_Hacker - I ONLY PORTED IT
    // https://www.unknowncheats.me/forum/counterstrike-global-offensive/173295-convars-checked-valve-servers.html
    public static class Bypass
    {
        public static bool WireframeEnabled                 = false;
        public static bool ThirdpersonEnabled               = false;
        public static bool MaterialGrayEnabled              = false;
        public static bool MaterialMinecraftEnabled         = false;
        public static bool ChromeEnabled                    = false;
        public static bool NoParticlesEnabled               = false;

        private static bool lastWireframeEnabled            = false;
        private static bool lastThirdpersonEnabled          = false;
        private static bool lastMaterialGrayEnabled         = false;
        private static bool lastMaterialMinecraftEnabled    = false;
        private static bool lastChromeEnabled               = false;
        private static bool lastNoParticlesEnabled          = false;

        static double Plat_FloatTime()
        {
            long counter;
            Windows.QueryPerformanceCounter(out counter);

            long start_time = M.Read<long>(Offsets.Plat_FloatTime_StartTime);
            double mult = M.Read<double>(Offsets.Plat_FloatTime_Multiplier);

            return (counter - start_time) * mult;
        }

        public static void Run()
        {
            while (true)
            {
                double curtime = Plat_FloatTime();

                M.Write<double>(curtime, Offsets.ClientThink_s_flOptimalCheck);
                M.Write<double>(curtime, Offsets.ClientThink_s_flOptimalNotification);

                if (WireframeEnabled != lastWireframeEnabled)
                {
                    M.Write(WireframeEnabled ? 2 : 1, Offsets.r_drawothermodels);
                    lastWireframeEnabled = WireframeEnabled;
                }

                if (ThirdpersonEnabled != lastThirdpersonEnabled)
                {
                    M.Write(ThirdpersonEnabled ? 1 : 0, Offsets.sv_cheats);
                    M.Write(ThirdpersonEnabled ? 1 : 0, Offsets.thirdperson);
                    lastThirdpersonEnabled = ThirdpersonEnabled;
                }

                if (MaterialGrayEnabled != lastMaterialGrayEnabled)
                {
                    M.Write(MaterialGrayEnabled ? 1 : 0, Offsets.mat_drawgray);
                    lastMaterialGrayEnabled = MaterialGrayEnabled;
                }

                if (MaterialMinecraftEnabled != lastMaterialMinecraftEnabled)
                {
                    M.Write(MaterialMinecraftEnabled ? 1 : 0, Offsets.mat_showlowresimage);
                    lastMaterialMinecraftEnabled = MaterialMinecraftEnabled;
                }

                if (ChromeEnabled != lastChromeEnabled)
                {
                    M.Write(ChromeEnabled ? 1 : 0, Offsets.r_showenvcubemap);
                    lastChromeEnabled = ChromeEnabled;
                }

                if (NoParticlesEnabled != lastNoParticlesEnabled)
                {
                    M.Write(NoParticlesEnabled ? 0 : 1, Offsets.r_drawparticles);
                    lastNoParticlesEnabled = NoParticlesEnabled;
                }

                Thread.Sleep(1);
            }
        }
    }
}
