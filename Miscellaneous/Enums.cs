using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control
{
    #region Flags
    [Flags]
    public enum MovementFlag
    {
        IN_AIR = 0,
        IN_AIR_DUCKING = 6,
        ON_GROUND = 1,
        ON_GROUND_DUCKING = 7,
    }
    [Flags]
    public enum ProcessAccessFlags : uint
    {
        All = 0x001F0FFF,
        Terminate = 0x00000001,
        CreateThread = 0x00000002,
        VirtualMemoryOperation = 0x00000008,
        VirtualMemoryRead = 0x00000010,
        VirtualMemoryWrite = 0x00000020,
        DuplicateHandle = 0x00000040,
        CreateProcess = 0x000000080,
        SetQuota = 0x00000100,
        SetInformation = 0x00000200,
        QueryInformation = 0x00000400,
        QueryLimitedInformation = 0x00001000,
        Synchronize = 0x00100000
    }
    [Flags]
    public enum ScanFlags
    {
        NONE,
        READ,
        SUBSTRACT_BASE,
    }
    [Flags]
    public enum Protection : uint
    {
        PAGE_NOACCESS = 0x01,
        PAGE_READONLY = 0x02,
        PAGE_READWRITE = 0x04,
        PAGE_WRITECOPY = 0x08,
        PAGE_EXECUTE = 0x10,
        PAGE_EXECUTE_READ = 0x20,
        PAGE_EXECUTE_READWRITE = 0x40,
        PAGE_EXECUTE_WRITECOPY = 0x80,
        PAGE_GUARD = 0x100,
        PAGE_NOCACHE = 0x200,
        PAGE_WRITECOMBINE = 0x400
    }
    #endregion

    #region Modes
    public enum AimbotCurveMode
    {
        LINEAR,
        UNDER,
        OVER
    }
    public enum BoxMode
    {
        Outline,
        Edge
    }
    public enum SkinChangerMode
    {
        AUTO,
        MANUAL,
        NOSLEEP
    }
    public enum AimbotMode
    {
        ANGLES,
        MOUSE
    }
    public enum LogMode
    {
        PARENT,
        CHILD
    }
    public enum TriggerMode
    {
        NoLegs,
        ChestOnly,
        HeadOnly
    }
    public enum GlowType
    {
        Outline,
        Bloom
    }
    public enum FovType
    {
        Distance,
        Normal
    }
    #endregion

    #region Miscellaneous
    public enum GameState
    {
        MENU = 0,
        GAME = 6
    }
    public enum Team
    {
        NONE,
        Spectator,
        Terrorist,
        Counter_Terrorist
    }
    public enum LifeState
    {
        Alive,
        Dead,
        Spectating
    }
    public enum Bone
    {
        Head = 6,
        Neck = 5,

        Chest = 4,
        Stomach = 3,

        Pelvis = 0
    }
    public enum ClassID
    {
        AK47 = 1,
        BaseAnimating = 2,
        BaseDoor = 10,
        BaseEntity = 11,
        BaseTrigger = 20,
        C4 = 28,
        CSGameRulesProxy = 33,
        CSPlayer = 34,
        CSPlayerResource = 35,
        CSRagdoll = 36,
        CSTeam = 37,
        CascadeLight = 29,
        Chicken = 30,
        ColorCorrection = 31,
        DEagle = 38,
        DecoyGrenade = 39,
        DynamicProp = 42,
        EnvDetailController = 50,
        EnvTonemapController = 57,
        EnvWind = 58,
        Flashbang = 63,
        FogController = 64,
        FuncBrush = 69,
        FuncOccluder = 74,
        FuncRotating = 76,
        Func_Dust = 66,
        HEGrenade = 81,
        Hostage = 82,
        IncendiaryGrenade = 84,
        Inferno = 85,
        Knife = 88,
        KnifeGG = 88,
        LightGlow = 90,
        MolotovGrenade = 92,
        ParticleSystem = 97,
        PhysicsProp = 100,
        PhysicsPropMultiplayer = 101,
        PlantedC4 = 103,
        PostProcessController = 109,
        PredictedViewModel = 112,
        PropDoorRotating = 114,
        RopeKeyframe = 120,
        ShadowControl = 123,
        SmokeGrenade = 125,
        SmokeStack = 126,
        Sprite = 129,
        Sun = 134,
        VGuiScreen = 190,
        VoteController = 191,
        WeaponAUG = 194,
        WeaponAWP = 195,
        WeaponBizon = 196,
        WeaponElite = 200,
        WeaponFiveSeven = 202,
        WeaponG3SG1 = 203,
        WeaponGalilAR = 205,
        WeaponGlock = 206,
        WeaponHKP2000 = 207,
        WeaponM249 = 208,
        WeaponM4A1 = 210,
        WeaponMP7 = 214,
        WeaponMP9 = 215,
        WeaponMag7 = 212,
        WeaponNOVA = 217,
        WeaponNegev = 216,
        WeaponP250 = 219,
        WeaponP90 = 220,
        WeaponSCAR20 = 222,
        WeaponSG556 = 226,
        WeaponSSG08 = 227,
        WeaponTaser = 228,
        WeaponTec9 = 229,
        WeaponUMP45 = 231,
        WeaponUMP45x = 232,
        WeaponXM1014 = 233,
        WeaponM4 = 211,
        WeaponNova = 218,
        WeaponMAG = 213,
        ParticleSmokeGrenade = 237,
        ParticleDecoy = 40,
        ParticleFlash = 9,
        ParticleIncendiaryGrenade = 93,
        WeaponG3SG1x = 204,
        WeaponDualBerettas = 201,
        WeaponTec9x = 230,
        WeaponPPBizon = 197,
        WeaponP90x = 221,
        WeaponSCAR20x = 223,
        WeaponXM1014x = 234,
        WeaponM249x = 209,
    }
    public enum WeaponClass
    {
        OTHER,
        HEAVY,
        SMG,
        RIFLE,
        SNIPER,
        PISTOL,
        KNIFE
    }
    public enum ItemDefinitionIndex
    {
        DEAGLE = 1,
        BERETTAS = 2,
        FIVESEVEN = 3,
        GLOCK = 4,
        AK47 = 7,
        AUG = 8,
        AWP = 9,
        FAMAS = 10,
        G3SG1 = 11,
        GALILAR = 13,
        M249 = 14,
        M4A4 = 16,
        MAC10 = 17,
        P90 = 19,
        UMP45 = 24,
        XM1014 = 25,
        BIZON = 26,
        MAG7 = 27,
        NEGEV = 28,
        SAWEDOFF = 29,
        TEC9 = 30,
        TASER = 31,
        P2000 = 32,
        MP7 = 33,
        MP9 = 34,
        NOVA = 35,
        P250 = 36,
        SCAR20 = 38,
        SG556 = 39,
        SSG08 = 40,
        KNIFE = 42,
        FLASHBANG = 43,
        HEGRENADE = 44,
        SMOKEGRENADE = 45,
        MOLOTOV = 46,
        DECOY = 47,
        INCGRENADE = 48,
        C4 = 49,
        KNIFE_T = 59,
        M4A1S = 60,
        USPS = 61,
        CZ75A = 63,
        REVOLVER = 64,
        KNIFE_BAYONET = 500,
        KNIFE_FLIP = 505,
        KNIFE_GUT = 506,
        KNIFE_KARAMBIT = 507,
        KNIFE_M9_BAYONET = 508,
        KNIFE_TACTICAL = 509,
        KNIFE_FALCHION = 512,
        KNIFE_SURVIVAL_BOWIE = 514,
        KNIFE_BUTTERFLY = 515,
        KNIFE_PUSH = 516
    }
    public enum GlowColor
    {
        Cyan,
        White,
        Green,
        Red,
        Magenta,
        Gold
    }
    public enum AllBones : int
    {
        pelvis = 0,
        pine_0 = 1,
        pine_1 = 2,
        pine_2 = 3,
        pine_3 = 4,
        eck_0 = 5,
        ead_0 = 6,
        lavicle_L = 7,
        rm_upper_L = 8,
        rm_lower_L = 9,
        hand_L = 10,
        finger_middle_meta_L = 11,
        finger_middle_0_L = 12,
        finger_middle_1_L = 13,
        finger_middle_2_L = 14,
        finger_pinky_meta_L = 15,
        finger_pinky_0_L = 16,
        finger_pinky_1_L = 17,
        finger_pinky_2_L = 18,
        finger_index_meta_L = 19,
        finger_index_0_L = 20,
        finger_index_1_L = 21,
        finger_index_2_L = 22,
        finger_thumb_0_L = 23,
        finger_thumb_1_L = 24,
        finger_thumb_2_L = 25,
        finger_ring_meta_L = 26,
        finger_ring_0_L = 27,
        finger_ring_1_L = 28,
        finger_ring_2_L = 29,
        weapon_hand_L = 30,
        arm_lower_L_TWIST = 31,
        arm_lower_L_TWIST1 = 32,
        arm_upper_L_TWIST = 33,
        arm_upper_L_TWIST1 = 34,
        clavicle_R = 35,
        arm_upper_R = 36,
        arm_lower_R = 37,
        hand_R = 38,
        finger_middle_meta_R = 39,
        finger_middle_0_R = 40,
        finger_middle_1_R = 41,
        finger_middle_2_R = 42,
        finger_pinky_meta_R = 43,
        finger_pinky_0_R = 44,
        finger_pinky_1_R = 45,
        finger_pinky_2_R = 46,
        finger_index_meta_R = 47,
        finger_index_0_R = 48,
        finger_index_1_R = 49,
        finger_index_2_R = 50,
        finger_thumb_0_R = 51,
        finger_thumb_1_R = 52,
        finger_thumb_2_R = 53,
        finger_ring_meta_R = 54,
        finger_ring_0_R = 55,
        finger_ring_1_R = 56,
        finger_ring_2_R = 57,
        weapon_hand_R = 58,
        arm_lower_R_TWIST = 59,
        arm_lower_R_TWIST1 = 60,
        arm_upper_R_TWIST = 61,
        arm_upper_R_TWIST1 = 62,
        leg_upper_L = 63,
        leg_lower_L = 4,
        ankle_L = 65,
        ball_L = 66,
        leg_upper_L_TWIST = 67,
        leg_upper_L_TWIST1 = 68,
        leg_upper_R = 69,
        leg_lower_R = 70,
        ankle_R = 71,
        ball_R = 72,
        leg_upper_R_TWIST = 73,
        leg_upper_R_TWIST1 = 74,
        ValveBiped_weapon_bone = 75,
        lh_ik_driver = 76,
        lean_root = 77,
        lfoot_lock = 78,
        rfoot_lock = 79,
        primary_jiggle_jnt = 80,
        primary_smg_jiggle_jnt = 81
    }
    #endregion
}
