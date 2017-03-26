using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Control
{
    public static class H
    {
        private static bool consoleEnabled = true;

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, out Rectangle lpRect);
        
        private static Random rnd = new Random();
        public static Memory Memory;
        public static SigScanner SigScanner;

        #region WinApi
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(Keys vKey);
        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(
            IntPtr hProcess,
            int lpBaseAddress,
            byte[] lpBuffer,
            int nSize,
            ref int lpNumberOfBytesRead
        );
        #endregion

        #region Miscellaneous Helpers
        public static bool KeyPressed(Keys key) => GetAsyncKeyState(key) != 0;
        public static void SetMousePos(Vector2 cursorPos) => Cursor.Position = new Point((int)cursorPos.X, (int)cursorPos.Y);
        public static void Log(string logMessage, LogMode mode)
        {
            if (!consoleEnabled)
                return;

            if (mode == LogMode.PARENT)
            {
                ConsoleColor oldColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(logMessage);
                Console.ForegroundColor = oldColor;
            }
            else // mode == LogMode.CHILD
            {
                ConsoleColor oldColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"  |_{logMessage}");
                Console.ForegroundColor = oldColor;
            }
        }
        public static void ResetConvars()
        {
            M.Write<byte>(1, Offsets.r_drawothermodels);
            M.Write<byte>(0, Offsets.thirdperson);
            M.Write<byte>(0, Offsets.sv_cheats);
            M.Write<byte>(0, Offsets.mat_showlowresimage);
            M.Write<byte>(0, Offsets.r_showenvcubemap);
            M.Write<byte>(1, Offsets.r_drawparticles);
        }
        public static void SafeExit()
        {
            H.SendPacket = true;
            ResetConvars();
            Environment.Exit(0);
        }
        #endregion

        #region Cheat Helpers

        #region Visuals
        public static bool ToScreen(this Vector3 point, out Vector2 screenPos)
        {
            Rectangle screenRectangle;
            GetWindowRect(G.GlobalOffensive.MainWindowHandle, out screenRectangle);

            screenPos = MathUtils.WorldToScreen(G.ViewMatrix, screenRectangle, point);

            if (screenPos == new Vector2(0, 0))
                return false;

            return true;
        }
        #endregion

        #region Aimbot
        public static Vector3 ViewAngles
        {
            get
            {
                return Memory.Read<Vector3>(G.ClientState() + Offsets.m_dwViewAngles);
            }

            set
            {
                Memory.Write(value, G.ClientState() + Offsets.m_dwViewAngles);
            }
        }
        public static double GetDistance(Vector3 targetPos, Vector3 localPlayerPos, Vector3 ViewAngleDifference)
        {
            if (ViewAngleDifference.Length() > 100)
                return 180;

            float playerDistance = Vector3.Distance(targetPos, localPlayerPos);
            double fYawDifference = Math.Sin(Math.Abs(ViewAngleDifference.Y).ToRadians()) * playerDistance;
            double fPitchDifference = Math.Sin(Math.Abs(ViewAngleDifference.X).ToRadians()) * playerDistance;
            return Math.Sqrt(fPitchDifference * fPitchDifference + fYawDifference * fYawDifference);
        }
        #endregion

        public static CSPlayer GetPlayer(int index)
        {
            int entitySize = (index - 1) * 0x10;
            return new CSPlayer(G.ClientBase + Offsets.m_dwEntityList + entitySize);
        }
        public static GameState GameState
        {
            get
            {
                return (GameState)M.Read<byte>(G.ClientState() + 0x100);
            }
        }
        public static bool SendPacket
        {
            get
            {
                return Convert.ToBoolean(M.Read<byte>(Offsets.bSendPacket));
            }
            set
            {
                M.Write<byte>(Convert.ToByte(value), Offsets.bSendPacket);
            }
        }
        public static void ForceUpdate() => M.Write<int>(-1, G.ClientState() + Offsets.ForceFullUpdate);
        public static IntPtr GetClientEntityFromHandle(IntPtr handle)
        {
            int weaponIndex = M.Read<int>(handle) & 0xFFF;
            return M.Read<IntPtr>((G.ClientBase + Offsets.m_dwEntityList + weaponIndex * 0x10) - 0x10);
        }
        #endregion

        #region RNG Helpers
        public static int RandomInt(int min, int max = int.MaxValue) => rnd.Next(min, max);
        public static float RandomFloat
        {
            get
            {
                return (float)rnd.NextDouble();
            }
        }
        #endregion
    }
}
