using System;

namespace Control
{
    class Offsets
    {
        #region DT_BasePlayer
        public static int m_iHealth         { get; set; }
        public static int m_lifeState       { get; set; }
        public static int m_vecVelocity     { get; set; }
        public static int m_hMyWeapons      { get; set; }
        public static int m_vecViewOffset   { get; set; }
        public static int m_fFlags          { get; set; }
        public static int m_hViewModel      { get; set; }
        public static int m_nModelIndex     { get; set; }
        #endregion

        #region BaseEntity
        public static int m_bDormant        = 0xE9;
        public static int m_dwBoneMatrix { get; set; }
        #endregion

        #region DT_BaseEntity
        public static int m_iTeamNum        { get; set; }
        public static int m_bSpotted        { get; set; }
        public static int m_bSpottedByMask  { get; set; }
        public static int m_vecMaxs         { get; set; }
        public static int m_hActiveWeapon   { get; set; }
        public static int m_vecOrigin       { get; set; }
        #endregion

        #region DT_CSPlayer
        public static int m_flFlashDuration { get; set; }
        public static int m_iShotsFired     { get; set; }
        public static int m_ArmorValue      { get; set; }
        public static int m_iGlowIndex      { get; set; }
        #endregion

        #region DT_BaseCombatWeapon
        public static int m_Item                    { get; set; }
        public static int m_iItemDefinitionIndex    { get; set; }
        public static int m_iClip1                  { get; set; }
        public static int m_zoomLevel               { get; set; }
        public static int m_iWeaponID               { get; set; }
        public static int m_nFallbackPaintKit       { get; set; }
        public static int m_nFallbackSeed           { get; set; }
        public static int m_flFallbackWear          { get; set; }
        public static int m_nFallbackStatTrak       { get; set; }
        public static int m_OriginalOwnerXuidLow    { get; set; }
        public static int m_OriginalOwnerXuidHigh   { get; set; }
        public static int m_iEntityQuality          { get; set; }
        public static int m_iItemIDHigh             { get; set; }
        public static int m_iItemIDLow              { get; set; }
        public static int m_szCustomName            { get; set; }
        #endregion

        #region DT_LOCAL
        public static int m_vecPunch { get; set; }
        #endregion

        #region ClientState
        public static int m_dwClientState { get; set; }
        public static int m_dwViewAngles { get; set; }
        #endregion

        #region NetVars
        public static IntPtr ClientClassesHead { get; set; }
        #endregion

        #region Extra
        public static IntPtr bSendPacket    { get; set; }
        public static int m_dwViewMatrix    { get; set; }
        public static int m_dwGlowObject    { get; set; }
        public static int m_dwForceJump     { get; set; }
        public static int m_dwForceAttack   { get; set; }
        public static int m_dwForceLeft     { get; set; }
        public static int m_dwForceRight    { get; set; }
        public static int m_dwEntityList    { get; set; }
        public static int ForceFullUpdate   { get; set; }
        public static IntPtr TeleportOffset { get; set; }

        public static IntPtr sv_cheats          { get; set; }
        public static IntPtr thirdperson        { get; set; }
        public static IntPtr mat_drawgray       { get; set; }
        public static IntPtr mat_showlowresimage  { get; set; }
        public static IntPtr r_showenvcubemap   { get; set; }
        public static IntPtr r_drawparticles { get; set; }

        public static IntPtr r_drawothermodels { get; set; }

        public static IntPtr Plat_FloatTime_StartTime               { get; set; }
        public static IntPtr Plat_FloatTime_Multiplier              { get; set; }
        public static IntPtr ClientThink_s_flOptimalCheck           { get; set; }
        public static IntPtr ClientThink_s_flOptimalNotification    { get; set; }
        #endregion

        #region LocalPlayer
        public static int m_dwLocalPlayer { get; set; }
        #endregion
    }
}
