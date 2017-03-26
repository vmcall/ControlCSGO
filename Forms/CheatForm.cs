using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Control
{
    public partial class CheatForm : Form
    {

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,      // x-coordinate of upper-left corner
            int nTopRect,       //y-coordinate of upper-left corner
            int nRightRect,     // x-coordinate of lower-right corner
            int nBottomRect,    // y-coordinate of lower-right corner
            int nWidthEllipse,  // height of ellipse
            int nHeightEllipse  //  width of ellipse
        );

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void TopPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        public CheatForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 5, 5));
            tabControl1.SelectedTab.BorderStyle = BorderStyle.None;
        }

        private void CheatForm_Load(object sender, EventArgs e)
        {
            //AppDomain.CurrentDomain.UnhandledException += delegate (object eventSender, UnhandledExceptionEventArgs eventArgs)
            //{
            //    H.SafeExit();
            //};

            AttachToGlobalOffensive();

            G.GlobalOffensive.Exited += delegate (object eventSender, EventArgs eventArgs)
            {
                Environment.Exit(0);
            };

            H.Memory = new Memory(G.GlobalOffensive);

            SetupModules();

            H.SigScanner = new SigScanner(G.GlobalOffensive.Handle);

            GrabOffsets();
            GrabNetvars();

            SetupPointers();

            Cheats.Manage.UpdateThread();

            SetupSelectedIndexes();
        }

        private void CheatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            H.SafeExit();
        }

        private static void GrabOffsets()
        {
            Offsets.sv_cheats = G.EngineBase + 0x5C08E0;
            Offsets.r_showenvcubemap = G.EngineBase + 0x5BBB18;
            Offsets.mat_drawgray = G.MaterialSystemBase + 0xB8030;
            Offsets.mat_showlowresimage = G.MaterialSystemBase + 0xB81E8;
            Offsets.thirdperson = G.ClientBase + 0x4E99835;
            Offsets.r_drawparticles = G.ClientBase + 0xA3A480;

            H.Log("Harcoded Offsets", LogMode.PARENT);
            H.Log($"sv_cheats -> {Offsets.sv_cheats.Hex()}", LogMode.CHILD);
            H.Log($"mat_drawgray -> {Offsets.mat_drawgray.Hex()}", LogMode.CHILD);
            H.Log($"thirdperson -> {Offsets.thirdperson.Hex()}", LogMode.CHILD);
            H.Log($"mat_showlowresimage -> {Offsets.mat_showlowresimage.Hex()}", LogMode.CHILD);
            H.Log($"r_showenvcubemap -> {Offsets.r_showenvcubemap.Hex()}", LogMode.CHILD);
            H.Log($"r_drawparticles -> {Offsets.r_drawparticles.Hex()}", LogMode.CHILD);

            Console.WriteLine(string.Empty);

            int PatternDumpStart;

            #region Tier0.dll
            if (H.SigScanner.DumpModule(G.Tier0Module))
            {
                PatternDumpStart = Environment.TickCount;

                H.Log("Dump Start - tier0.dll", LogMode.PARENT);

                IntPtr ptr = (IntPtr)H.SigScanner.FindPattern(
                    "2B 0D ? ? ? ? 8B 55 FC 1B 15 ? ? ? ? E8 ? ? ? ? F2 0F 59 05 ? ? ? ? F2 0F 11 45 F8",
                    ScanFlags.NONE,
                    0, 0);

                new Thread(() => 
                {
                    Offsets.Plat_FloatTime_StartTime = M.Read<IntPtr>(ptr + 2);

                    H.Log($"Plat_FloatTime_StartTime -> {Offsets.Plat_FloatTime_StartTime.Hex()} - Elapsed: {Environment.TickCount - PatternDumpStart}ms", LogMode.CHILD);

                }).Start();

                new Thread(() =>
                {
                    Offsets.Plat_FloatTime_Multiplier = M.Read<IntPtr>(ptr + 24);

                    H.Log($"Plat_FloatTime_Multiplier -> {Offsets.Plat_FloatTime_Multiplier.Hex()} - Elapsed: {Environment.TickCount - PatternDumpStart}ms", LogMode.CHILD);

                }).Start();

                do Thread.Sleep(1);
                while (Offsets.Plat_FloatTime_Multiplier == IntPtr.Zero || Offsets.Plat_FloatTime_StartTime == IntPtr.Zero);
            }
            #endregion

            Console.WriteLine(string.Empty);

            #region Engine.dll
            if (H.SigScanner.DumpModule(G.EngineModule))
            {
                H.Log("Dump Start - engine.dll", LogMode.PARENT);

                #region ClientState

                // m_dwClientState
                new Thread(() =>
                {
                    PatternDumpStart = Environment.TickCount;

                    Offsets.m_dwClientState = H.SigScanner.FindPattern(
                        "A1 ? ? ? ? F3 0F 11 80 ? ? ? ? D9 46 04 D9 05",
                        ScanFlags.READ | ScanFlags.SUBSTRACT_BASE,
                        1, 0); ;

                    H.Log($"m_dwClientState -> {Offsets.m_dwClientState.Hex()} - Elapsed: {Environment.TickCount - PatternDumpStart}ms", LogMode.CHILD);
                }).Start();

                // m_dwViewAngles
                new Thread(() =>
                {
                    PatternDumpStart = Environment.TickCount;

                    Offsets.m_dwViewAngles = H.SigScanner.FindPattern(
                        "F3 0F 11 80 ? ? ? ? D9 46 04 D9 05 ? ? ? ?",
                        ScanFlags.READ,
                        0x4, 0);

                    H.Log($"m_dwViewAngles -> {Offsets.m_dwViewAngles.Hex()} - Elapsed: {Environment.TickCount - PatternDumpStart}ms", LogMode.CHILD);
                }).Start();
                #endregion

                #region Extra
                // bSendPacket
                new Thread(() =>
                {
                    PatternDumpStart = Environment.TickCount;
                    Offsets.bSendPacket = (IntPtr)H.SigScanner.FindPattern("B3 1 8B 1 8B 40 10 FF D0", ScanFlags.NONE, 0x1, 0);
                    H.Log($"bSendPacket -> {Offsets.bSendPacket.Hex()} - Elapsed: {Environment.TickCount - PatternDumpStart}ms", LogMode.CHILD);
                }).Start();

                // ForceFullUpdate
                new Thread(() =>
                {
                    PatternDumpStart = Environment.TickCount;
                    Offsets.ForceFullUpdate = H.SigScanner.FindPattern("B0 FF B7 ? ? ? ? E8", ScanFlags.READ, 3, 0);

                    H.Log($"ForceFullUpdate -> {(Offsets.ForceFullUpdate).Hex()} - Elapsed: {Environment.TickCount - PatternDumpStart}ms", LogMode.CHILD);
                }).Start();
                #endregion

                do Thread.Sleep(1);
                while (
                Offsets.m_dwClientState == 0 ||
                Offsets.m_dwViewAngles == 0 ||
                Offsets.ForceFullUpdate == 0 ||
                Offsets.bSendPacket == IntPtr.Zero);
            }
            else
            {
                MessageBox.Show("ERROR: EngineModuleDumpFailed - Contact Sleek");
                Environment.Exit(0);
            }
            #endregion

            Console.WriteLine(string.Empty);

            #region Client.dll
            if (H.SigScanner.DumpModule(G.ClientModule))
            {

                H.Log("Dump Start - client.dll", LogMode.PARENT);

                // m_bDormant
                new Thread(() =>
                {
                    PatternDumpStart = Environment.TickCount;
                    Offsets.m_bDormant = H.SigScanner.FindPattern(
                        "88 9E ? ? ? ? E8 ? ? ? ? 53 8D 8E",
                        ScanFlags.READ,
                        0x2, 0);
                    H.Log($"m_bDormant -> {Offsets.m_bDormant.Hex()} - Elapsed: {Environment.TickCount - PatternDumpStart}ms", LogMode.CHILD);
                }).Start();

                // ClientClassesHead
                new Thread(() =>
                {
                    PatternDumpStart = Environment.TickCount;
                    int address = H.SigScanner.FindPattern("44 54 5F 54 45 57 6F", ScanFlags.NONE, 0, 0);

                    List<string> ClientClassesHeadPatternList = new List<string>();

                    foreach (byte Byte in BitConverter.GetBytes(address))
                        ClientClassesHeadPatternList.Add(Byte.ToString("X"));

                    string ClientClassesHeadPattern = string.Join(" ", ClientClassesHeadPatternList);

                    Offsets.ClientClassesHead = (IntPtr)H.SigScanner.FindPattern(ClientClassesHeadPattern, ScanFlags.READ, 0x2B, 0);

                    H.Log($"ClientClassesHead -> {Offsets.ClientClassesHead.Hex()} <{ClientClassesHeadPattern}> - Elapsed: {Environment.TickCount - PatternDumpStart}ms", LogMode.CHILD);
                }).Start();

                // LocalPlayer
                new Thread(() =>
                {
                    PatternDumpStart = Environment.TickCount;

                    Offsets.m_dwLocalPlayer = H.SigScanner.FindPattern(
                        "A3 ? ? ? ? C7 05 ? ? ? ? ? ? ? ? E8 ? ? ? ? 59 C3 6A",
                        ScanFlags.READ | ScanFlags.SUBSTRACT_BASE,
                        0x1, 0x10);

                    H.Log($"m_dwLocalPlayer -> {Offsets.m_dwLocalPlayer.Hex()} - Elapsed: {Environment.TickCount - PatternDumpStart}ms", LogMode.CHILD);
                }).Start();

                // m_dwViewMatrix
                new Thread(() =>
                {
                    PatternDumpStart = Environment.TickCount;

                    Offsets.m_dwViewMatrix = H.SigScanner.FindPattern(
                        "E8 ? ? ? ? 8D 95 E0",
                        ScanFlags.READ | ScanFlags.SUBSTRACT_BASE,
                         -0x1A, 0x90);

                    H.Log($"m_dwViewMatrix -> {Offsets.m_dwViewMatrix.Hex()} - Elapsed: {Environment.TickCount - PatternDumpStart}ms", LogMode.CHILD);
                }).Start();

                //  m_dwGlowObject
                new Thread(() =>
                {
                    PatternDumpStart = Environment.TickCount;

                    Offsets.m_dwGlowObject = H.SigScanner.FindPattern(
                        "A1 ? ? ? ? A8 01 75 4E 0F 57 C0",
                        ScanFlags.READ | ScanFlags.SUBSTRACT_BASE,
                        0x58, 0);

                    H.Log($"m_dwGlowObject -> {Offsets.m_dwGlowObject.Hex()} - Elapsed: {Environment.TickCount - PatternDumpStart}ms", LogMode.CHILD);
                }).Start();

                // m_dwForceJump
                new Thread(() =>
                {
                    PatternDumpStart = Environment.TickCount;

                    Offsets.m_dwForceJump = H.SigScanner.FindPattern(
                        "89 15 ? ? ? ? 8B 15 ? ? ? ? F6 C2 03 74 03 83 CE 08",
                        ScanFlags.READ | ScanFlags.SUBSTRACT_BASE,
                        0x2, 0);

                    H.Log($"m_dwForceJump -> {Offsets.m_dwForceJump.Hex()} - Elapsed: {Environment.TickCount - PatternDumpStart}ms", LogMode.CHILD);
                }).Start();

                //  m_dwForceLeft
                new Thread(() =>
                {
                    PatternDumpStart = Environment.TickCount;

                    Offsets.m_dwForceLeft = H.SigScanner.FindPattern("A8 80 BF", ScanFlags.READ | ScanFlags.SUBSTRACT_BASE, 0x44, 0);

                    H.Log($"m_dwForceLeft -> {Offsets.m_dwForceLeft.Hex()} - Elapsed: {Environment.TickCount - PatternDumpStart}ms", LogMode.CHILD);
                }).Start();

                //  m_dwForceRight
                new Thread(() =>
                {
                    PatternDumpStart = Environment.TickCount;

                    Offsets.m_dwForceRight = H.SigScanner.FindPattern("A8 80 BF", ScanFlags.READ | ScanFlags.SUBSTRACT_BASE, 0x6F, 0);

                    H.Log($"m_dwForceRight -> {Offsets.m_dwForceRight.Hex()} - Elapsed: {Environment.TickCount - PatternDumpStart}ms", LogMode.CHILD);
                }).Start();

                //  m_dwForceAttack
                new Thread(() =>
                {
                    PatternDumpStart = Environment.TickCount;

                    Offsets.m_dwForceAttack = H.SigScanner.FindPattern(
                        "89 15 ? ? ? ? 8B 15 ? ? ? ? F6 C2 03 74 03 83 CE 04",
                        ScanFlags.READ | ScanFlags.SUBSTRACT_BASE,
                        0x2, 0);

                    H.Log($"m_dwForceAttack -> {Offsets.m_dwForceAttack.Hex()} - Elapsed: {Environment.TickCount - PatternDumpStart}ms", LogMode.CHILD);
                }).Start();
                
                // m_dwEntityList
                new Thread(() =>
                {
                    PatternDumpStart = Environment.TickCount;

                    Offsets.m_dwEntityList = H.SigScanner.FindPattern(
                        "BB ? ? ? ? 83 FF 01 0F 8C ? ? ? ? 3B F8",
                        ScanFlags.READ | ScanFlags.SUBSTRACT_BASE,
                        0x1, 0);

                    H.Log($"m_dwEntityList -> {Offsets.m_dwEntityList.Hex()} - Elapsed: {Environment.TickCount - PatternDumpStart}ms", LogMode.CHILD);
                }).Start();

                // Teleport Offset
                new Thread(() =>
                {
                    PatternDumpStart = Environment.TickCount;

                    Offsets.TeleportOffset = (IntPtr)H.SigScanner.FindPattern("BE ? ? ? ? 2B F1 3B F3 7D 1F 8B 55 0C 8B C3", ScanFlags.NONE, 0, 0);

                    H.Log($"Teleport -> {Offsets.TeleportOffset.Hex()} - Elapsed: {Environment.TickCount - PatternDumpStart}ms", LogMode.CHILD);
                }).Start();

                // ClientThink_s_flOptimalCheck
                new Thread(() => 
                {
                    PatternDumpStart = Environment.TickCount;
                    IntPtr pointer = (IntPtr)H.SigScanner.FindPattern("8B 07 8B CF F2 0F 11 15 ? ? ? ? 8B 80 ? ? ? ? FF D0 83 F8 03 74 09", ScanFlags.NONE, 0, 0);
                    Offsets.ClientThink_s_flOptimalCheck = M.Read<IntPtr>(pointer + 8);
                    H.Log($"ClientThink_s_flOptimalCheck -> {Offsets.ClientThink_s_flOptimalCheck.Hex()} - Elapsed: {Environment.TickCount - PatternDumpStart}ms", LogMode.CHILD);
                }).Start();

                // ClientThink_s_flOptimalNotification
                new Thread(() => {
                    PatternDumpStart = Environment.TickCount;
                    IntPtr pointer = (IntPtr)H.SigScanner.FindPattern("51 8D 4C 24 ? F2 0F 11 1D ? ? ? ? E8 ? ? ? ? 8B 4C 24", ScanFlags.NONE, 0, 0);
                    Offsets.ClientThink_s_flOptimalNotification = M.Read<IntPtr>(pointer + 9);
                    H.Log($"ClientThink_s_flOptimalNotification -> {Offsets.ClientThink_s_flOptimalNotification.Hex()} - Elapsed: {Environment.TickCount - PatternDumpStart}ms", LogMode.CHILD);

                }).Start();

                // r_drawothermodels
                new Thread(() => {
                    PatternDumpStart = Environment.TickCount;
                    IntPtr pointer = (IntPtr)H.SigScanner.FindPattern("8B 0D ? ? ? ? 33 DB 81 F9 ? ? ? ? 75 07 A1 ? ? ? ? EB 05", ScanFlags.NONE, 0, 0);
                    Offsets.r_drawothermodels = M.Read<IntPtr>(pointer + 17);
                    H.Log($"r_drawothermodels -> {Offsets.r_drawothermodels.Hex()} - Elapsed: {Environment.TickCount - PatternDumpStart}ms", LogMode.CHILD);

                }).Start();

                do Thread.Sleep(1);
                while (
                Offsets.m_bDormant == 0 ||
                Offsets.m_dwLocalPlayer == 0 ||
                Offsets.m_dwViewMatrix == 0 ||
                Offsets.m_dwGlowObject == 0 ||
                Offsets.m_dwForceJump == 0 ||
                Offsets.m_dwEntityList == 0 ||
                Offsets.m_dwForceAttack == 0 ||
                Offsets.m_dwForceLeft == 0 ||
                Offsets.m_dwForceRight == 0 ||
                Offsets.ClientClassesHead == IntPtr.Zero ||
                Offsets.TeleportOffset == IntPtr.Zero || 
                Offsets.ClientThink_s_flOptimalCheck == IntPtr.Zero || 
                Offsets.ClientThink_s_flOptimalNotification == IntPtr.Zero || 
                Offsets.r_drawothermodels == IntPtr.Zero);
            }
            else
            {
                MessageBox.Show("ERROR: ClientModuleDumpFailed - Contact Sleek");
                Environment.Exit(0);
            }
            #endregion
        }

        private static void GrabNetvars()
        {
            Console.WriteLine(String.Empty);
            H.Log("Netvars", LogMode.PARENT);

            #region DT_BasePlayer
            Offsets.m_iHealth = NetvarManager.GetOffset("DT_BasePlayer", "m_iHealth");
            Offsets.m_lifeState = NetvarManager.GetOffset("DT_BasePlayer", "m_lifeState");
            Offsets.m_vecVelocity = NetvarManager.GetOffset("DT_BasePlayer", "m_vecVelocity[0]");
            Offsets.m_hActiveWeapon = NetvarManager.GetOffset("DT_BasePlayer", "m_hActiveWeapon");
            Offsets.m_hMyWeapons = NetvarManager.GetOffset("DT_BasePlayer", "m_hMyWeapons");
            Offsets.m_fFlags = NetvarManager.GetOffset("DT_BasePlayer", "m_fFlags");
            Offsets.m_vecViewOffset = NetvarManager.GetOffset("DT_BasePlayer", "m_vecViewOffset[0]");
            Offsets.m_hViewModel = NetvarManager.GetOffset("DT_BasePlayer", "m_hViewModel[0]");
            Offsets.m_nModelIndex = NetvarManager.GetOffset("DT_BasePlayer", "m_nModelIndex");
            #endregion

            #region BaseEntity
            Offsets.m_dwBoneMatrix = NetvarManager.GetOffset("DT_BaseAnimating", "m_nForceBone", 0x1C);
            #endregion

            #region DT_BaseEntity
            Offsets.m_iTeamNum = NetvarManager.GetOffset("DT_BaseEntity", "m_iTeamNum");
            Offsets.m_bSpotted = NetvarManager.GetOffset("DT_BaseEntity", "m_bSpotted");
            Offsets.m_bSpottedByMask = NetvarManager.GetOffset("DT_BaseEntity", "m_bSpottedByMask");
            Offsets.m_vecMaxs = NetvarManager.GetOffset("DT_BaseEntity", "m_Collision", 0x14);
            Offsets.m_vecOrigin = NetvarManager.GetOffset("DT_BaseEntity", "m_vecOrigin");
            #endregion

            #region DT_CSPlayer
            Offsets.m_flFlashDuration = NetvarManager.GetOffset("DT_CSPlayer", "m_flFlashDuration");
            Offsets.m_iShotsFired = NetvarManager.GetOffset("DT_CSPlayer", "m_iShotsFired");
            Offsets.m_ArmorValue = NetvarManager.GetOffset("DT_CSPlayer", "m_ArmorValue");
            Offsets.m_iGlowIndex = Offsets.m_flFlashDuration + 0x18;
            #endregion

            #region DT_BaseCombatWeapon
            Offsets.m_iClip1 = NetvarManager.GetOffset("DT_BaseCombatWeapon", "m_iClip1");
            Offsets.m_zoomLevel = NetvarManager.GetOffset("DT_WeaponCSBaseGun", "m_zoomLevel");
            Offsets.m_iWeaponID = NetvarManager.GetOffset("DT_WeaponCSBase", "m_fAccuracyPenalty", 0x2C);
            Offsets.m_Item =
                NetvarManager.GetOffset("DT_BaseCombatWeapon", "m_Item",
                NetvarManager.GetOffset("DT_BaseCombatWeapon", "m_AttributeManager"));

            Offsets.m_nFallbackPaintKit = NetvarManager.GetOffset("DT_BaseCombatWeapon", "m_nFallbackPaintKit");
            Offsets.m_nFallbackSeed = NetvarManager.GetOffset("DT_BaseCombatWeapon", "m_nFallbackSeed");
            Offsets.m_flFallbackWear = NetvarManager.GetOffset("DT_BaseCombatWeapon", "m_flFallbackWear");
            Offsets.m_nFallbackStatTrak = NetvarManager.GetOffset("DT_BaseCombatWeapon", "m_nFallbackStatTrak");
            Offsets.m_OriginalOwnerXuidLow = NetvarManager.GetOffset("DT_BaseCombatWeapon", "m_OriginalOwnerXuidLow");
            Offsets.m_OriginalOwnerXuidHigh = NetvarManager.GetOffset("DT_BaseCombatWeapon", "m_OriginalOwnerXuidHigh");
            Offsets.m_iEntityQuality = NetvarManager.GetOffset("DT_BaseCombatWeapon", "m_iEntityQuality");
            Offsets.m_iItemIDHigh = NetvarManager.GetOffset("DT_BaseCombatWeapon", "m_iItemIDHigh");
            Offsets.m_iItemIDLow = NetvarManager.GetOffset("DT_BaseCombatWeapon", "m_iItemIDLow");
            Offsets.m_iItemDefinitionIndex = NetvarManager.GetOffset("DT_BaseCombatWeapon", "m_iItemDefinitionIndex");
            Offsets.m_szCustomName = NetvarManager.GetOffset("DT_BaseCombatWeapon", "m_szCustomName");
            #endregion

            #region DT_Local
            Offsets.m_vecPunch = NetvarManager.GetOffset("DT_BasePlayer", "m_Local", 0x70);
            #endregion
        }

        private static void AttachToGlobalOffensive()
        {
            try { G.GlobalOffensive = Process.GetProcessesByName("csgo")[0]; }
            catch
            {
                MessageBox.Show("ERROR: GlobalOffensive Attachment Failed", "CS:GO NOT FOUND", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
        }

        private static void SetupPointers()
        {
            G.LocalPlayer = new CSPlayer(G.ClientBase + Offsets.m_dwLocalPlayer);
            G.GlowPointer = M.Read<IntPtr>(G.ClientBase + Offsets.m_dwGlowObject);
        }

        private static void SetupModules()
        {
            G.ClientModule = H.Memory.GetModule("client.dll");
            G.EngineModule = H.Memory.GetModule("engine.dll");
            G.MaterialSystemModule = H.Memory.GetModule("materialsystem.dll");
            G.Tier0Module = H.Memory.GetModule("tier0.dll");
        }

        private void SetupSelectedIndexes()
        {
            cmbBhopMode.SelectedIndex = 0;
            cmbAimBone.SelectedIndex = 0;
            cmbAimKey.SelectedIndex = 0;
            cmbAimCurveMode.SelectedIndex = 0;
            cmbGlowColor.SelectedIndex = 0;
            cmbEspBoxMode.SelectedIndex = 0;
            cmbSkinChangerMode.SelectedIndex = 0;
            cmbAimbotMode.SelectedIndex = 0;
            cmbTriggerKey.SelectedIndex = 0;
            cmbTriggerMode.SelectedIndex = 0;
            cmbGlowType.SelectedIndex = 0;
            cmbAimFovType.SelectedIndex = 0;
        }

        #region Aimbot
        private void numAimSmooth_MouseDown(object sender, MouseEventArgs e)
        {
            Cheats.Aimbot.Smooth = (float)numAimSmooth.Value;
        }
        private void numAimFov_MouseDown(object sender, MouseEventArgs e)
        {
            Cheats.Aimbot.Fov = (float)numAimFov.Value;
        }

        private void numRcsCompensationX_MouseDown(object sender, MouseEventArgs e)
        {
            Cheats.Aimbot.RcsCompensationX = (float)numRcsCompensationX.Value;
        }
        private void numRcsCompensationY_MouseDown(object sender, MouseEventArgs e)
        {
            Cheats.Aimbot.RcsCompensationY = (float)numRcsCompensationY.Value;
        }
        private void numRcsStart_MouseDown(object sender, MouseEventArgs e)
        {
            Cheats.Aimbot.RcsRequiredBullets = (int)numRcsStart.Value;
        }
        private void cmbAimMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cheats.Aimbot.AimMode = (AimbotMode)cmbAimbotMode.SelectedIndex;
        }
        private void cmbAimCurveMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cheats.Aimbot.CurveMode = (AimbotCurveMode)cmbAimCurveMode.SelectedIndex;
        }
        private void chkAimbot_CheckedChanged(object sender)
        {
            Cheats.Manage.ToggleAimbot(chkAimbot.Checked);
        }
        private void cmbAimFovType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cheats.Aimbot.FovType = (FovType)cmbAimFovType.SelectedIndex;
        }
        private void cmbAimBone_SelectedIndexChanged(object sender, EventArgs e)
        {
            Bone bone = Bone.Head;

            switch (cmbAimBone.SelectedIndex)
            {
                case 0:
                    bone = Bone.Head;
                    break;

                case 1:
                    bone = Bone.Neck;
                    break;

                case 2:
                    bone = Bone.Chest;
                    break;

                case 3:
                    bone = Bone.Stomach;
                    break;

                case 4:
                    bone = Bone.Pelvis;
                    break;
            }

            Cheats.Aimbot.AimBone = bone;
        }
        private void cmbAimKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            Keys key = Keys.LButton;

            switch (cmbAimKey.SelectedIndex)
            {
                case 0:
                    key = Keys.XButton1;
                    break;

                case 1:
                    key = Keys.XButton2;
                    break;

                case 2:
                    key = Keys.LButton;
                    break;
            }

            Cheats.Aimbot.ToggleKey = key;
        }
        #endregion

        #region Triggerbot
        private void chkTrigger_CheckedChanged(object sender)
        {
            Cheats.Manage.ToggleTriggerbot(chkTrigger.Checked);
        }

        private void cmbTriggerKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbTriggerKey.SelectedIndex)
            {
                case 0:
                    Cheats.Triggerbot.Triggerbutton = Keys.Alt;
                    break;

                case 1:
                    Cheats.Triggerbot.Triggerbutton = Keys.XButton1;
                    break;

                case 2:
                    Cheats.Triggerbot.Triggerbutton = Keys.XButton2;
                    break;
            }
        }
        private void cmbTriggerMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cheats.Triggerbot.Mode = (TriggerMode)cmbTriggerKey.SelectedIndex;
        }

        private void numTriggerDelay_MouseDown(object sender, MouseEventArgs e)
        {
            Cheats.Triggerbot.Delay = (int)numTriggerDelay.Value;
        }
        #endregion

        #region Bunnyhop
        private void cmbBhopMode_SelectedIndexChanged(object sender, EventArgs e) => Cheats.Bunnyhop.BunnyhopMode = (Cheats.BhopMode)cmbBhopMode.SelectedIndex;
        
        private void numBhopHops_MouseDown(object sender, MouseEventArgs e) => Cheats.Bunnyhop.MaxJumps = (int)numBhopHops.Value;
        
        private void chkBhopEnabled_CheckedChanged(object sender) => Cheats.Manage.ToggleBunnyhop(chkBhop.Checked);
        private void chkAutoStrafe_CheckedChanged(object sender) => Cheats.Bunnyhop.AutoStrafe = chkAutoStrafe.Checked;
        #endregion

        #region Skin Changer
        private void chkSkinChanger_CheckedChanged(object sender)
        {
            Cheats.Manage.ToggleSkinChanger(chkSkinChanger.Checked);
        }

        private void btnForceFullUpdate_Click(object sender, EventArgs e)
        {
            H.ForceUpdate();
        }

        private void cmbSkinChangerMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cheats.Skinchanger.SkinChangerMode = (SkinChangerMode)cmbSkinChangerMode.SelectedIndex;
        }
        #endregion

        #region Miscellaneous
        private void chkNoFlash_CheckedChanged(object sender)
        {
            Cheats.Manage.ToggleNoFlash(chkNoFlash.Checked);
        }
        private void chkLag_CheckedChanged(object sender)
        {
            Cheats.Manage.ToggleLag(chkLag.Checked);
        }
        private void trackLagAmount_Scroll(object sender, EventArgs e)
        {
            Cheats.Misc.LagAmount = trackLagAmount.Value + 1;
        }
        private void btnTeleport_Click(object sender, EventArgs e)
        {
            Cheats.Teleport.Activate();
        }
        #endregion

        #region Visuals
        private void chkEspGlow_CheckedChanged(object sender)
        {
            Cheats.Visuals._GlowEnabled = chkEspGlow.Checked;
        }

        private void chkEsp_CheckedChanged(object sender)
        {
            Cheats.Manage.ToggleEsp(chkEsp.Checked);
        }

        private void chkEspRadar_CheckedChanged(object sender)
        {
            Cheats.Visuals._RadarEnabled = chkEspRadar.Checked;
        }

        private void chkEspInfo_CheckedChanged(object sender)
        {
            Cheats.Visuals._InfoEnabled = chkEspInfo.Checked;
        }
        private void chkEspHealth_CheckedChanged(object sender)
        {
            Cheats.Visuals._HealthEnabled = chkEspHealth.Checked;
        }
        private void chkEspBox_CheckedChanged(object sender)
        {
            Cheats.Visuals._BoxEnabled = chkEspBox.Checked;
        }
        private void cmbEspBoxMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cheats.Visuals._BoxMode = (BoxMode)cmbEspBoxMode.SelectedIndex;
        }
        private void cmbGlowColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((GlowColor)cmbGlowColor.SelectedIndex)
            {
                case GlowColor.Cyan:
                    Cheats.Visuals._GlowColor = Color.Cyan;
                    break;

                case GlowColor.White:
                    Cheats.Visuals._GlowColor = Color.White;
                    break;
                    
                case GlowColor.Green:
                    Cheats.Visuals._GlowColor = Color.Green;
                    break;

                case GlowColor.Red:
                    Cheats.Visuals._GlowColor = Color.Red;
                    break;

                case GlowColor.Magenta:
                    Cheats.Visuals._GlowColor = Color.Magenta;
                    break;

                case GlowColor.Gold:
                    Cheats.Visuals._GlowColor = Color.Gold;
                    break;
            }
        }
        private void cmbGlowType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cheats.Visuals._GlowType = (GlowType)cmbGlowType.SelectedIndex;
        }
        private void trackGlowIntensity_Scroll(object sender, EventArgs e)
        {
            Cheats.Visuals._GlowIntensity = (float)trackGlowIntensity.Value * 10;
        }
        #endregion

        #region UI
        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
            Environment.Exit(Environment.ExitCode);
        }

        private void AimAssistButton_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
            AimAssistButton.BackColor = Color.FromArgb(255, 33, 52, 76);
            SkinChangerButton.BackColor = Color.FromArgb(255, 37, 62, 93);
            EspButton.BackColor = Color.FromArgb(255, 37, 62, 93);
            BunnyHopButton.BackColor = Color.FromArgb(255, 37, 62, 93);
            TriggerBotButton.BackColor = Color.FromArgb(255, 37, 62, 93);
            MiscButton.BackColor = Color.FromArgb(255, 37, 62, 93);
        }

        private void SkinChangerButton_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
            SkinChangerButton.BackColor = Color.FromArgb(255, 33, 52, 76);
            AimAssistButton.BackColor = Color.FromArgb(255, 37, 62, 93);
            EspButton.BackColor = Color.FromArgb(255, 37, 62, 93);
            BunnyHopButton.BackColor = Color.FromArgb(255, 37, 62, 93);
            TriggerBotButton.BackColor = Color.FromArgb(255, 37, 62, 93);
            MiscButton.BackColor = Color.FromArgb(255, 37, 62, 93);

        }

        private void BunnyHopButton_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
            BunnyHopButton.BackColor = Color.FromArgb(255, 33, 52, 76);
            AimAssistButton.BackColor = Color.FromArgb(255, 37, 62, 93);
            EspButton.BackColor = Color.FromArgb(255, 37, 62, 93);
            SkinChangerButton.BackColor = Color.FromArgb(255, 37, 62, 93);
            TriggerBotButton.BackColor = Color.FromArgb(255, 37, 62, 93);
            MiscButton.BackColor = Color.FromArgb(255, 37, 62, 93);

        }

        private void EspButton_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 3;
            EspButton.BackColor = Color.FromArgb(255, 33, 52, 76);
            AimAssistButton.BackColor = Color.FromArgb(255, 37, 62, 93);
            SkinChangerButton.BackColor = Color.FromArgb(255, 37, 62, 93);
            BunnyHopButton.BackColor = Color.FromArgb(255, 37, 62, 93);
            TriggerBotButton.BackColor = Color.FromArgb(255, 37, 62, 93);
            MiscButton.BackColor = Color.FromArgb(255, 37, 62, 93);

        }

        private void TriggerBotButton_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 4;
            TriggerBotButton.BackColor = Color.FromArgb(255, 33, 52, 76);
            AimAssistButton.BackColor = Color.FromArgb(255, 37, 62, 93);
            SkinChangerButton.BackColor = Color.FromArgb(255, 37, 62, 93);
            BunnyHopButton.BackColor = Color.FromArgb(255, 37, 62, 93);
            EspButton.BackColor = Color.FromArgb(255, 37, 62, 93);
            MiscButton.BackColor = Color.FromArgb(255, 37, 62, 93);

        }

        private void MiscButton_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 5;
            MiscButton.BackColor = Color.FromArgb(255, 33, 52, 76);
            AimAssistButton.BackColor = Color.FromArgb(255, 37, 62, 93);
            SkinChangerButton.BackColor = Color.FromArgb(255, 37, 62, 93);
            BunnyHopButton.BackColor = Color.FromArgb(255, 37, 62, 93);
            EspButton.BackColor = Color.FromArgb(255, 37, 62, 93);
            TriggerBotButton.BackColor = Color.FromArgb(255, 37, 62, 93);
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        #endregion

        #region Bypass
        private void chkBypassEnabled_CheckedChanged(object sender) => Cheats.Manage.ToggleBypass(chkBypassEnabled.Checked);

        private void chkThirdpersonEnabled_CheckedChanged(object sender) => Cheats.Bypass.ThirdpersonEnabled = chkThirdpersonEnabled.Checked;
        private void chkWireframeEnabled_CheckedChanged(object sender) => Cheats.Bypass.WireframeEnabled = chkWireframeEnabled.Checked;
        private void chkGrayMaterialsEnabled_CheckedChanged(object sender) => Cheats.Bypass.MaterialGrayEnabled = chkGrayMaterialsEnabled.Checked;
        private void chkMinecraftMaterials_CheckedChanged(object sender) => Cheats.Bypass.MaterialMinecraftEnabled = chkMinecraftMaterials.Checked;
        private void chkChromeModels_CheckedChanged(object sender) => Cheats.Bypass.ChromeEnabled = chkChromeModels.Checked;
        #endregion

        private void btnKnifeChangerApply_Click(object sender, EventArgs e)
        {
            if (!txtKnifeChangerSkin.Text.DigitsOnly())
                return;

            if (!txtKnifeChangerModel.Text.DigitsOnly())
                return;

            Cheats.Skinchanger.KnifeModel = Int32.Parse(txtKnifeChangerModel.Text);
            Cheats.Skinchanger.KnifeSkin = Int32.Parse(txtKnifeChangerSkin.Text);

            switch (cmbKnifeIndex.SelectedIndex)
            {
                case 0:
                    Cheats.Skinchanger.KnifeIndex = ItemDefinitionIndex.KNIFE_BAYONET;
                    break;

                case 1:
                    Cheats.Skinchanger.KnifeIndex = ItemDefinitionIndex.KNIFE_FLIP;
                    break;

                case 2:
                    Cheats.Skinchanger.KnifeIndex = ItemDefinitionIndex.KNIFE_GUT;
                    break;

                case 3:
                    Cheats.Skinchanger.KnifeIndex = ItemDefinitionIndex.KNIFE_KARAMBIT;
                    break;

                case 4:
                    Cheats.Skinchanger.KnifeIndex = ItemDefinitionIndex.KNIFE_M9_BAYONET;
                    break;

                case 5:
                    Cheats.Skinchanger.KnifeIndex = ItemDefinitionIndex.KNIFE_TACTICAL;
                    break;

                case 6:
                    Cheats.Skinchanger.KnifeIndex = ItemDefinitionIndex.KNIFE_FALCHION;
                    break;

                case 7:
                    Cheats.Skinchanger.KnifeIndex = ItemDefinitionIndex.KNIFE_SURVIVAL_BOWIE;
                    break;

                case 8:
                    Cheats.Skinchanger.KnifeIndex = ItemDefinitionIndex.KNIFE_BUTTERFLY;
                    break;

                case 9:
                    Cheats.Skinchanger.KnifeIndex = ItemDefinitionIndex.KNIFE_PUSH;
                    break;

            }
        }

        private void chkNoSmoke_CheckedChanged(object sender) => Cheats.Bypass.NoParticlesEnabled = chkNoParticles.Checked;
        
    }
}