using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Control.Cheats
{
    public static class Aimbot
    {
        public static float RcsCompensationX = 2;
        public static float RcsCompensationY = 2;
        public static int RcsRequiredBullets = 0;

        public static Keys ToggleKey = Keys.LButton;
        public static Bone AimBone = Bone.Head;
        public static AimbotCurveMode CurveMode = AimbotCurveMode.LINEAR;
        public static AimbotMode AimMode = AimbotMode.ANGLES;
        public static FovType FovType = FovType.Distance;
        public static float Smooth = 1;
        public static float Fov = 3;

        public static void Run()
        {
            CSPlayer BestEntity = null;

            while (true)
            {
                Thread.Sleep(1);

                if (!G.GlobalOffensive.Active())
                    continue;

                Vector3 eyePosition = G.LocalPlayer.EyePosition;
                Vector3 viewAngles = H.ViewAngles;

                if (H.KeyPressed(ToggleKey) && BestEntity != null)
                {
                    if (BestEntity.InAir || !BestEntity.Alive)
                    {
                        BestEntity = null;
                        continue;
                    }

                    WeaponClass localWeaponClass = G.LocalPlayer.Weapon.Class;

                    if (localWeaponClass == WeaponClass.OTHER)
                    {
                        BestEntity = null;
                        continue;
                    }

                    Vector3 targetAngle = BestEntity.BoneAngles(AimBone, eyePosition, viewAngles);
                    Vector3 aimPunchAngle = G.LocalPlayer.AimPunch;

                    targetAngle /= Smooth;
                    aimPunchAngle /= Smooth;
                    int shotsFired = G.LocalPlayer.ShotsFired;

                    switch (CurveMode)
                    {
                        case AimbotCurveMode.UNDER:
                            targetAngle = targetAngle.UnderCompensate();
                            break;

                        case AimbotCurveMode.OVER:
                            targetAngle = targetAngle.OverCompensate();
                            break;
                    }

                    if (shotsFired >= Aimbot.RcsRequiredBullets && localWeaponClass != WeaponClass.PISTOL && localWeaponClass != WeaponClass.SNIPER)
                        targetAngle -= new Vector3(aimPunchAngle.X * RcsCompensationX, aimPunchAngle.Y * RcsCompensationY, 0);

                    Vector3 smoothedTargetAngle = targetAngle / Smooth;

                    switch (AimMode)
                    {
                        case AimbotMode.ANGLES:
                            H.ViewAngles += smoothedTargetAngle;
                            break;

                        case AimbotMode.MOUSE:
                            //Vector2 screenPos = new Vector2();

                            //Vector3 worldPos = smoothedTargetAngle.ToWorld();

                            //worldPos.ToScreen(out screenPos);

                            //H.SetMousePos(screenPos);

                            break;
                    }
                }
                else
                {
                    double bestDistance = 360;
                    double distance = 0;
                    BestEntity = null;
                    Vector3 localPlayerPos = G.LocalPlayer.Position;

                    foreach (var Player in new List<CSPlayer>(G.TargetList))
                    {
                        if (Player == null || !Player.SpottedByMask)
                            continue;

                        Vector3 boneAngles = Player.BoneAngles(AimBone, eyePosition, viewAngles);

                        if (FovType == FovType.Distance)
                            distance = H.GetDistance(Player.Position, localPlayerPos, boneAngles);
                        else
                            distance = boneAngles.Length();

                        if (distance < bestDistance && distance <= Fov)
                        {
                            bestDistance = distance;
                            BestEntity = Player;
                        }
                    }
                }
            }
        }
    }
}
