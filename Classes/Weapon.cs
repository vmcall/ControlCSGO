using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control
{
    public class BaseCombatWeapon
    {
        IntPtr entPointer;

        public BaseCombatWeapon(IntPtr weaponPointer)
        {
            this.entPointer = weaponPointer;
        }

        public ItemDefinitionIndex ItemDefinitionIndex
        {
            get
            {
                return (ItemDefinitionIndex)M.Read<int>(this.entPointer + Offsets.m_Item + Offsets.m_iItemDefinitionIndex);
            }
            set
            {
                M.Write((int)value, this.entPointer + Offsets.m_Item + Offsets.m_iItemDefinitionIndex);
            }
        }
        public int Id
        {
            get
            {
                return M.Read<int>(this.entPointer + Offsets.m_iWeaponID);
            }
        }
        public int Ammo
        {
            get
            {
                return M.Read<int>(this.entPointer + Offsets.m_iClip1);
            }
        }
        public int ZoomLevel
        {
            get
            {
                return M.Read<int>(this.entPointer + Offsets.m_zoomLevel);
            }
        }
        
        public int ItemIDHigh
        {
            get
            {
                return M.Read<int>(entPointer + Offsets.m_Item + Offsets.m_iItemIDHigh);
            }

            set
            {
                M.Write<int>(value, entPointer + Offsets.m_Item + Offsets.m_iItemIDHigh);
            }
        }
        public int ItemIDLow
        {
            get
            {
                return M.Read<int>(entPointer + Offsets.m_Item + Offsets.m_iItemIDLow);
            }

            set
            {
                M.Write<int>(value, entPointer + Offsets.m_Item + Offsets.m_iItemIDLow);
            }
        }
        public int PaintKit
        {
            get
            {
                return M.Read<int>(entPointer + Offsets.m_nFallbackPaintKit);
            }

            set
            {
                M.Write<int>(value, entPointer + Offsets.m_nFallbackPaintKit);
            }
        }
        public int Seed
        {
            get
            {
                return M.Read<int>(entPointer+ Offsets.m_nFallbackSeed);
            }

            set
            {
                M.Write<int>(value, entPointer + Offsets.m_nFallbackSeed);
            }
        }
        public int StatTrak
        {
            get
            {
                return M.Read<int>(entPointer + Offsets.m_nFallbackStatTrak);
            }

            set
            {
                M.Write<int>(value, entPointer + Offsets.m_nFallbackStatTrak);
            }
        }
        public int XuidLow
        {
            get
            {
                return M.Read<int>(entPointer + Offsets.m_OriginalOwnerXuidLow);
            }

            set
            {
                M.Write<int>(value, entPointer + Offsets.m_OriginalOwnerXuidLow);
            }
        }
        public int XuidHigh
        {
            get
            {
                return M.Read<int>(entPointer + Offsets.m_OriginalOwnerXuidHigh);
            }

            set
            {
                M.Write<int>(value, entPointer + Offsets.m_OriginalOwnerXuidHigh);
            }
        }
        public int Quality
        {
            get
            {
                return M.Read<int>(entPointer + Offsets.m_Item + Offsets.m_iEntityQuality);
            }

            set
            {
                M.Write<int>(value, entPointer + Offsets.m_Item + Offsets.m_iEntityQuality);
            }
        }
        public float Wear
        {
            get
            {
                return M.Read<float>(entPointer + Offsets.m_flFallbackWear);
            }

            set
            {
                M.Write<float>(value, entPointer + Offsets.m_flFallbackWear);
            }
        }
        public string Name
        {
            get
            {
                switch (this.ItemDefinitionIndex)
                {
                    case ItemDefinitionIndex.DEAGLE:
                        return "Desert Eagle";
                    case ItemDefinitionIndex.BERETTAS:
                        return "Dual Berettas";
                    case ItemDefinitionIndex.FIVESEVEN:
                        return "Five-SeveN";
                    case ItemDefinitionIndex.GLOCK:
                        return "Glock-18";
                    case ItemDefinitionIndex.AK47:
                        return "AK-47";
                    case ItemDefinitionIndex.AUG:
                        return "AUG";
                    case ItemDefinitionIndex.AWP:
                        return "AWP";
                    case ItemDefinitionIndex.FAMAS:
                        return "FAMAS";
                    case ItemDefinitionIndex.G3SG1:
                        return "G3SG1";
                    case ItemDefinitionIndex.GALILAR:
                        return "Galil";
                    case ItemDefinitionIndex.M249:
                        return "M249";
                    case ItemDefinitionIndex.M4A4:
                        return "M4A1";
                    case ItemDefinitionIndex.MAC10:
                        return "MAC-10";
                    case ItemDefinitionIndex.P90:
                        return "P90";
                    case ItemDefinitionIndex.UMP45:
                        return "UMP-45";
                    case ItemDefinitionIndex.XM1014:
                        return "XM1014";
                    case ItemDefinitionIndex.BIZON:
                        return "PP-Bizon";
                    case ItemDefinitionIndex.MAG7:
                        return "MAG-7";
                    case ItemDefinitionIndex.NEGEV:
                        return "Negev";
                    case ItemDefinitionIndex.SAWEDOFF:
                        return "Sawed-Off";
                    case ItemDefinitionIndex.TEC9:
                        return "Tec-9";
                    case ItemDefinitionIndex.TASER:
                        return "Taser";
                    case ItemDefinitionIndex.P2000:
                        return "P2000";
                    case ItemDefinitionIndex.MP7:
                        return "MP7";
                    case ItemDefinitionIndex.MP9:
                        return "MP9";
                    case ItemDefinitionIndex.NOVA:
                        return "Nova";
                    case ItemDefinitionIndex.P250:
                        return "P250";
                    case ItemDefinitionIndex.SCAR20:
                        return "SCAR-20";
                    case ItemDefinitionIndex.SG556:
                        return "SG 553";
                    case ItemDefinitionIndex.SSG08:
                        return "SSG 08";
                    case ItemDefinitionIndex.KNIFE:
                        return "Knife";
                    case ItemDefinitionIndex.FLASHBANG:
                        return "Flashbang";
                    case ItemDefinitionIndex.HEGRENADE:
                        return "HE Grenade";
                    case ItemDefinitionIndex.SMOKEGRENADE:
                        return "Smoke Grenade";
                    case ItemDefinitionIndex.MOLOTOV:
                        return "Molotov";
                    case ItemDefinitionIndex.DECOY:
                        return "Decoy";
                    case ItemDefinitionIndex.INCGRENADE:
                        return "Incendiary Grenade";
                    case ItemDefinitionIndex.C4:
                        return "C4";
                    case ItemDefinitionIndex.KNIFE_T:
                        return "Knife";
                    case ItemDefinitionIndex.M4A1S:
                        return "M4A1-S";
                    case ItemDefinitionIndex.USPS:
                        return "USP-S";
                    case ItemDefinitionIndex.CZ75A:
                        return "CZ75-Auto";
                    case ItemDefinitionIndex.REVOLVER:
                        return "R8 Revolver";
                    default:
                        return "Knife";
                }
            }
            set
            {
                M.WriteString(value, entPointer + Offsets.m_Item + Offsets.m_szCustomName);
            }
        }
        public WeaponClass Class
        {
            get
            {
                switch (this.ItemDefinitionIndex)
                {
                    case ItemDefinitionIndex.DEAGLE:
                    case ItemDefinitionIndex.BERETTAS:
                    case ItemDefinitionIndex.FIVESEVEN:
                    case ItemDefinitionIndex.GLOCK:
                    case ItemDefinitionIndex.TEC9:
                    case ItemDefinitionIndex.USPS:
                    case ItemDefinitionIndex.P2000:
                    case ItemDefinitionIndex.P250:
                    case ItemDefinitionIndex.CZ75A:
                    case ItemDefinitionIndex.REVOLVER:
                        return WeaponClass.PISTOL;

                    // Rifles
                    case ItemDefinitionIndex.AK47:
                    case ItemDefinitionIndex.AUG:
                    case ItemDefinitionIndex.FAMAS:
                    case ItemDefinitionIndex.GALILAR:
                    case ItemDefinitionIndex.M4A4:
                    case ItemDefinitionIndex.M4A1S:
                    case ItemDefinitionIndex.SG556:
                        return WeaponClass.RIFLE;

                    // Snipers
                    case ItemDefinitionIndex.AWP:
                    case ItemDefinitionIndex.G3SG1:
                    case ItemDefinitionIndex.SCAR20:
                    case ItemDefinitionIndex.SSG08:
                        return WeaponClass.SNIPER;
                    // Heavy
                    case ItemDefinitionIndex.M249:
                    case ItemDefinitionIndex.XM1014:
                    case ItemDefinitionIndex.MAG7:
                    case ItemDefinitionIndex.NEGEV:
                    case ItemDefinitionIndex.SAWEDOFF:
                    case ItemDefinitionIndex.NOVA:
                        return WeaponClass.HEAVY;

                    // SMGs
                    case ItemDefinitionIndex.MAC10:
                    case ItemDefinitionIndex.P90:
                    case ItemDefinitionIndex.UMP45:
                    case ItemDefinitionIndex.BIZON:
                    case ItemDefinitionIndex.MP7:
                    case ItemDefinitionIndex.MP9:
                        return WeaponClass.SMG;

                    case ItemDefinitionIndex.KNIFE:
                    case ItemDefinitionIndex.KNIFE_T:
                    case ItemDefinitionIndex.KNIFE_BAYONET:
                    case ItemDefinitionIndex.KNIFE_FLIP:
                    case ItemDefinitionIndex.KNIFE_GUT:
                    case ItemDefinitionIndex.KNIFE_KARAMBIT:
                    case ItemDefinitionIndex.KNIFE_M9_BAYONET:
                    case ItemDefinitionIndex.KNIFE_TACTICAL:
                    case ItemDefinitionIndex.KNIFE_FALCHION:
                    case ItemDefinitionIndex.KNIFE_SURVIVAL_BOWIE:
                    case ItemDefinitionIndex.KNIFE_BUTTERFLY:
                    case ItemDefinitionIndex.KNIFE_PUSH:
                        return WeaponClass.KNIFE;
                    default:
                        return WeaponClass.OTHER;
                }
            }
        }
    }
}
