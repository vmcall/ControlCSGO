using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Control
{
    public static class VectorExtensions
    {
        public static Vector3 Safe(this Vector3 vec)
        {
            if (vec.X > 180)
                vec.X -= 360;

            else if (vec.X < -180)
                vec.X += 360;

            if (vec.Y > 180)
                vec.Y -= 360;

            else if (vec.Y < -180)
                vec.Y += 360;

            if (vec.X < -89)
                vec.X = -89;

            if (vec.X > 89)
                vec.X = 89;

            while (vec.Y < -180.0f)
                vec.Y += 360.0f;

            while (vec.Y > 180.0f)
                vec.Y -= 360.0f;

            vec.Z = 0;

            return vec;
        }
        public static Vector3 UnderCompensate(this Vector3 vec)
        {
            if (vec.Y > .3 || vec.Y < -.3)
                vec.X += H.RandomFloat / 5;

            else if (vec.Y > .2 || vec.Y < -.2)
                vec.X += H.RandomFloat / 7;

            else if (vec.Y > .1 || vec.Y < -.1)
                vec.X += H.RandomFloat / 15;

            else if (vec.Y > .07 || vec.Y < -.07)
                vec.X += H.RandomFloat / 17;

            else if (vec.Y > .05 || vec.Y < -.05)
                vec.X += H.RandomFloat / 18;

            else if (vec.Y > .03 || vec.Y < -.03)
                vec.X += H.RandomFloat / 22;

            return vec;
        }
        public static Vector3 OverCompensate(this Vector3 vec)
        {
            if (vec.Y > .3 || vec.Y < -.3)
                vec.X -= H.RandomFloat / 5;

            else if (vec.Y > .2 || vec.Y < -.2)
                vec.X -= H.RandomFloat / 7;

            else if (vec.Y > .1 || vec.Y < -.1)
                vec.X -= H.RandomFloat / 15;

            else if (vec.Y > .07 || vec.Y < -.07)
                vec.X -= H.RandomFloat / 17;

            else if (vec.Y > .05 || vec.Y < -.05)
                vec.X -= H.RandomFloat / 18;

            else if (vec.Y > .03 || vec.Y < -.03)
                vec.X -= H.RandomFloat / 22;

            return vec;
        }
        public static Vector3 ToEulerAngles(this Vector3 angles)
        {
            float tmp, yaw, pitch;

            if (angles.Y == 0 && angles.X == 0)
            {
                yaw = 0;
                if (angles.Z > 0)
                    pitch = 270;
                else
                    pitch = 90;
            }
            else
            {
                yaw = (float)(Math.Atan2(angles.Y, angles.X) * 180 / Math.PI);
                if (yaw < 0)
                    yaw += 360;

                tmp = (float)Math.Sqrt(angles.X * angles.X + angles.Y * angles.Y);
                pitch = (float)(Math.Atan2(-angles.Z, tmp) * 180 / Math.PI);
                if (pitch < 0)
                    pitch += 360;
            }

            return new Vector3(pitch, yaw, 0);
        }
        public static Vector3 ToWorld(this Vector3 angles)
        {
            float sp, sy, cp, cy;

            sy = (float)Math.Cos(angles.X.ToRadians());
            cy = (float)Math.Cos(angles.Y.ToRadians());
            sp = (float)Math.Sin(angles.X.ToRadians());
            cp = (float)Math.Sin(angles.Y.ToRadians());

            return new Vector3(cp * cy, cp * sy, -sp);
        }
    }
}
