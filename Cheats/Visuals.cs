using System;
using System.Threading;
using System.Drawing;
using System.Linq;
using Control.Overlay;
using System.Numerics;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Control.Cheats
{
    public static class Visuals
    {
        public static bool _GlowEnabled { get; set; }
        public static bool _BoxEnabled { get; set; }
        public static bool _HealthEnabled { get; set; }
        public static bool _RadarEnabled { get; set; }
        public static bool _InfoEnabled { get; set; }

        public static float _GlowIntensity = 100;

        public static BoxMode _BoxMode = BoxMode.Outline;
        public static GlowType _GlowType = GlowType.Outline;

        public static Color _GlowColor = Color.Aqua;

        public static void Run()
        {
            OverlayWindow Overlay = new OverlayWindow(G.GlobalOffensive.MainWindowHandle, false);

            Overlay.Show();

            var consolasFont = Overlay.Graphics.CreateFont("Consolas", 15);
            var csgoFont = Overlay.Graphics.CreateFont("csgo_icons", 15);

            var brushWhite = Overlay.Graphics.CreateBrush(SharpDX.Color.White);
            var brushOrange = Overlay.Graphics.CreateBrush(SharpDX.Color.Orange);
            var brushRed = Overlay.Graphics.CreateBrush(SharpDX.Color.Red);
            var brushGrey = Overlay.Graphics.CreateBrush(SharpDX.Color.LightGray);

            while (true)
            {
                Thread.Sleep(1);

                if (!G.GlobalOffensive.Active())
                {
                    Overlay.Graphics.BeginScene();
                    Overlay.Graphics.ClearScene();
                    Overlay.Graphics.EndScene();
                    continue;
                }

                if (Visuals._InfoEnabled || Visuals._BoxEnabled || Visuals._HealthEnabled)
                    G.ViewMatrix = M.ReadMatrix(G.ClientBase + Offsets.m_dwViewMatrix, 4, 4);

                Overlay.Graphics.BeginScene();
                Overlay.Graphics.ClearScene();

                foreach (CSPlayer Player in new List<CSPlayer>(G.TargetList))
                {
                    if (Player == null || Player.Dead)
                        continue;

                    if (_RadarEnabled)
                        Player.Spotted = true;

                    if (_GlowEnabled)
                        Player.Glow(_GlowColor, _GlowType, _GlowIntensity);

                    if (_InfoEnabled || _BoxEnabled || _HealthEnabled)
                    {
                        Vector2 playerPosScreen, playerTopScreen = new Vector2();

                        Vector3 playerPos = Player.Position;
                        Vector3 playerTop = playerPos + new Vector3(0, 0, Player.VecMax.Z);

                        if (playerPos.ToScreen(out playerPosScreen) && playerTop.ToScreen(out playerTopScreen))
                        {
                            float height = playerPosScreen.Y - playerTopScreen.Y;
                            float width = height / 2;
                            float barWidth = height / 15;

                            if (_InfoEnabled)
                            {
                                int weaponId = Player.Weapon.Id;

                                if (weaponId > 500)
                                    continue;

                                string weaponIcon = Convert.ToChar(0xE000 + weaponId).ToString();
                                Overlay.Graphics.DrawText(weaponIcon, csgoFont, brushWhite, playerTopScreen.X - 15, playerTopScreen.Y - 20);
                            }

                            if (_HealthEnabled)
                            {
                                Overlay.Graphics.DrawBarHorizontal(
                                    (int)(playerPosScreen.X - width / 2),   // X
                                    (int)playerPosScreen.Y + 5,             // Y
                                    (int)width,                             // Width
                                    (int)height / 20,                       // Height
                                    Player.Health,                          // Value
                                    1,                                      // Stroke
                                    brushGrey,                              // Brush
                                    brushRed);                              // Interior Brush
                            }
                            
                            if (_BoxEnabled)
                                switch (_BoxMode)
                                {
                                    case BoxMode.Outline:
                                        Overlay.Graphics.DrawRectangle(playerTopScreen.X - width / 2, playerTopScreen.Y, width, height, 1, brushWhite);
                                        break;

                                    case BoxMode.Edge:
                                        Overlay.Graphics.DrawEdge(playerTopScreen.X - width / 2, playerTopScreen.Y, width, height, 5, 1, brushWhite);
                                        break;
                                }
                        }
                    }
                }

                Overlay.Graphics.EndScene();
            }
        }
    }
}
