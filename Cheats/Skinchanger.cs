using Control.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Control.Cheats
{
    public static class Skinchanger
    {
        public static SkinChangerMode SkinChangerMode = SkinChangerMode.AUTO;

        public static int KnifeModel = 0;
        public static int KnifeSkin = 0;
        public static ItemDefinitionIndex KnifeIndex = ItemDefinitionIndex.KNIFE;

        public static void Run()
        {
            while (true)
            {
                bool forceUpdate = false;

                if (KnifeModel > 0)
                {
                    BaseCombatWeapon wep = G.LocalPlayer.Weapon;

                    if (wep.Class == WeaponClass.KNIFE)
                        G.LocalPlayer.ViewModel.ModelIndex = KnifeModel;
                }

                foreach (BaseCombatWeapon weapon in G.LocalPlayer.Weapons)
                {
                    if (weapon.Class == WeaponClass.OTHER)
                        continue;

                    if (weapon.ItemIDHigh == -1)
                        continue;

                    weapon.ItemIDHigh = -1;
                    weapon.Wear = 0.00000000001F;

                    if (weapon.Class == WeaponClass.KNIFE && KnifeSkin > 0)
                    {
                        weapon.PaintKit = KnifeSkin;
                        weapon.StatTrak = 1337;
                        weapon.ItemDefinitionIndex = KnifeIndex;
                        continue;
                    }

                    switch (weapon.ItemDefinitionIndex)
                    {
                        #region Rifles
                        case ItemDefinitionIndex.AWP:
                            weapon.PaintKit = 344;
                            break;
                        case ItemDefinitionIndex.AK47:
                            weapon.PaintKit = 524;
                            break;
                        case ItemDefinitionIndex.M4A1S:
                            weapon.PaintKit = 548;
                            break;
                        case ItemDefinitionIndex.M4A4:
                            weapon.PaintKit = 309;
                            break;
                        case ItemDefinitionIndex.SSG08:
                            weapon.PaintKit = 222;
                            break;

                        case ItemDefinitionIndex.AUG:
                            weapon.PaintKit = 455;
                            break;

                        case ItemDefinitionIndex.FAMAS:
                            weapon.PaintKit = 455;
                            break;

                        case ItemDefinitionIndex.G3SG1:
                            weapon.PaintKit = 511;
                            break;

                        case ItemDefinitionIndex.GALILAR:
                            weapon.PaintKit = 398;
                            break;

                        case ItemDefinitionIndex.SCAR20:
                            weapon.PaintKit = 312;
                            break;
                        #endregion

                        #region SMG
                        case ItemDefinitionIndex.P90:
                            weapon.PaintKit = 359;
                            break;

                        case ItemDefinitionIndex.BIZON:
                            weapon.PaintKit = 13;
                            break;

                        case ItemDefinitionIndex.MAC10:
                            weapon.PaintKit = 433;
                            break;

                        case ItemDefinitionIndex.MP7:
                            weapon.PaintKit = 416;
                            break;

                        case ItemDefinitionIndex.MP9:
                            weapon.PaintKit = 33;
                            break;

                        case ItemDefinitionIndex.UMP45:
                            weapon.PaintKit = 37;
                            break;
                        #endregion

                        #region Heavy
                        case ItemDefinitionIndex.MAG7:
                            weapon.PaintKit = 431;
                            break;

                        case ItemDefinitionIndex.NOVA:
                            weapon.PaintKit = 62;
                            break;
                        #endregion

                        #region Pistols
                        case ItemDefinitionIndex.DEAGLE:
                            weapon.PaintKit = 527;
                            break;

                        case ItemDefinitionIndex.GLOCK:
                            weapon.PaintKit = 353;
                            break;

                        case ItemDefinitionIndex.USPS:
                            weapon.PaintKit = 504;
                            break;

                        case ItemDefinitionIndex.P2000:
                            weapon.PaintKit = 389;
                            break;

                        case ItemDefinitionIndex.CZ75A:
                            weapon.PaintKit = 270;
                            break;

                        case ItemDefinitionIndex.BERETTAS:
                            weapon.PaintKit = 249;
                            break;

                        case ItemDefinitionIndex.FIVESEVEN:
                            weapon.PaintKit = 510;
                            break;

                        case ItemDefinitionIndex.P250:
                            weapon.PaintKit = 258;
                            break;

                        case ItemDefinitionIndex.TEC9:
                            weapon.PaintKit = 520;
                            break;
                        #endregion
                    }
                    
                    forceUpdate = true;
                }


                if (forceUpdate && SkinChangerMode == SkinChangerMode.AUTO)
                    H.ForceUpdate();

                if (SkinChangerMode != SkinChangerMode.NOSLEEP)
                    Thread.Sleep(1);
            }
        }
    }
}
