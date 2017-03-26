using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Control.Cheats
{
    public static class Triggerbot
    {
        public static Keys Triggerbutton = Keys.Alt;
        public static TriggerMode Mode = TriggerMode.NoLegs;
        public static int Delay = 250;

        public static void Run()
        {
            while (true)
            {
                Thread.Sleep(1);

                if (!G.GlobalOffensive.Active())
                    continue;

                if (!H.KeyPressed(Triggerbutton))
                    continue;

                bool attack = false;

                Vector3 localEyePos = G.LocalPlayer.EyePosition;
                Vector3 viewAng = H.ViewAngles;

                foreach (CSPlayer Player in new List<CSPlayer>(G.TargetList))
                {
                    if (Player == null || !Player.Alive || attack)
                        continue;

                    Vector3 playerPosition = Player.Position;

                    Vector3 headAng, neckAng, chestAng;
                    double headDist, neckDist, chestDist;

                    switch (Mode)
                    {
                        case TriggerMode.NoLegs:
                            headAng = Player.BoneAngles(Bone.Head, localEyePos, viewAng);
                            headDist = H.GetDistance(playerPosition, localEyePos, headAng);

                            if (headDist <= 6)
                            {
                                attack = true;
                                continue;
                            }

                            neckAng = Player.BoneAngles(Bone.Neck, localEyePos, viewAng);
                            neckDist = H.GetDistance(playerPosition, localEyePos, neckAng);

                            if (neckDist <= 6)
                            {
                                attack = true;
                                continue;
                            }

                            chestAng = Player.BoneAngles(Bone.Chest, localEyePos, viewAng);
                            chestDist = H.GetDistance(playerPosition, localEyePos, chestAng);

                            if (chestDist <= 6)
                            {
                                attack = true;
                                continue;
                            }

                            break;

                        case TriggerMode.ChestOnly:
                            chestAng = Player.BoneAngles(Bone.Chest, localEyePos, viewAng);
                            chestDist = H.GetDistance(playerPosition, localEyePos, chestAng);

                            if (chestDist <= 5)
                            {
                                attack = true;
                                continue;
                            }

                            break;

                        case TriggerMode.HeadOnly:
                            headAng = Player.BoneAngles(Bone.Head, localEyePos, viewAng);
                            headDist = H.GetDistance(playerPosition, localEyePos, headAng);

                            if (headDist <= 5)
                            {
                                attack = true;
                                continue;
                            }

                            break;
                    }
                }

                if (attack)
                {
                    if (Delay > 0)
                        Thread.Sleep(Delay);

                    Controls.Attack();
                }

            }
        }
    }
}
