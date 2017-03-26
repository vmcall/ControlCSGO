using Control.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace Control
{
    public class CSPlayer
    {
        IntPtr entPointer;
        
        public CSPlayer(IntPtr entityAddress)
        {
            this.entPointer = M.Read<IntPtr>(entityAddress);
        }
        
        public IntPtr BoneMatrix
        {
            get
            {
                return M.Read<IntPtr>(this.entPointer + Offsets.m_dwBoneMatrix);
            }
        }
        public BaseCombatWeapon Weapon
        {
            get
            {
                return new BaseCombatWeapon(H.GetClientEntityFromHandle(this.entPointer + Offsets.m_hActiveWeapon));
            }
        }
        public List<BaseCombatWeapon> Weapons
        {
            get
            {
                List<BaseCombatWeapon> myWeapons = new List<BaseCombatWeapon>();

                for (int x = 0; x < 8; x++)
                {
                    IntPtr weaponPointer = H.GetClientEntityFromHandle(this.entPointer + Offsets.m_hMyWeapons + x * sizeof(int));
                    BaseCombatWeapon weapon = new BaseCombatWeapon(weaponPointer);
                    
                    if (weapon.ItemDefinitionIndex != 0 && weapon != null)
                        myWeapons.Add(weapon);
                }

                return myWeapons;
            }
        }

        public ViewModel ViewModel
        {
            get
            {
                return new ViewModel(H.GetClientEntityFromHandle(this.entPointer + Offsets.m_hViewModel));
            }
        }
        public int Index
        {
            get
            {
                return M.Read<int>(this.entPointer + 0x64);
            }
        }
        public int Health
        {
            get
            {
                return M.Read<int>(this.entPointer + Offsets.m_iHealth);
            }
        }
        public int ShotsFired
        {
            get
            {
                return M.Read<int>(this.entPointer + Offsets.m_iShotsFired);
            }
        }
        public int GlowIndex
        {
            get
            {
                return M.Read<int>(this.entPointer + Offsets.m_iGlowIndex);
            }
        }

        public float FlashDuration
        {
            get
            {
                return M.Read<float>(this.entPointer + Offsets.m_flFlashDuration);
            }

            set
            {
                M.Write(value, this.entPointer + Offsets.m_flFlashDuration);
            }
        }

        public bool Valid
        {
            get
            {
                if (this.Dormant)
                    return false;

                if (!this.Alive)
                    return false;

                if (this.Index == 0)
                    return false;

                if (this.Team == Team.NONE || this.Spectator)
                    return false;

                return true;
            }
        }
        public bool MovementFlag(MovementFlag flag) => this.MovementFlags.HasFlag(flag);
        public bool Alive
        {
            get
            {
                return this.LifeState == LifeState.Alive;
            }
        }
        public bool Dead
        {
            get
            {
                return this.LifeState == LifeState.Dead;
            }
        }
        public bool Teammate
        {
            get
            {
                return this.Team == G.LocalPlayer.Team;
            }
        }
        public bool Spectator
        {
            get
            {
                return this.Team == Team.Spectator;
            }
        }
        public bool InAir
        {
            get
            {
                return this.Velocity.Z > 0;
            }
        }
        public bool Spotted
        {
            get
            {
                return M.Read<bool>(this.entPointer + Offsets.m_bSpotted);
            }

            set
            {
                M.Write<byte>(Convert.ToByte(value), this.entPointer + Offsets.m_bSpotted);
            }
        }
        public bool Dormant
        {
            get
            {
                return M.Read<bool>(this.entPointer + Offsets.m_bDormant);
            }
        }
        public bool SpottedByMask
        {
            get
            {
                return M.Read<bool>(this.entPointer + Offsets.m_bSpottedByMask);

            }
        }

        public MovementFlag MovementFlags
        {
            get
            {
                return (MovementFlag)M.Read<byte>(this.entPointer + Offsets.m_fFlags);
            }
        }
        public LifeState LifeState
        {
            get
            {
                return (LifeState)M.Read<byte>(this.entPointer + Offsets.m_lifeState);
            }
        }
        public Team Team
        {
            get
            {
                return (Team)M.Read<byte>(this.entPointer + Offsets.m_iTeamNum);
            }
        }

        public void Glow(Color color, GlowType type, float intensity)
        {
            IntPtr glowAddress = G.GlowPointer + this.GlowIndex * 0x38;

            M.Write((float)color.R, glowAddress + 0x4);
            M.Write((float)color.G, glowAddress + 0x8);
            M.Write((float)color.B, glowAddress + 0xC);
            M.Write<float>(intensity / 100, glowAddress + 0x10);
            M.Write<byte>(1, glowAddress + 0x24);
            M.Write<byte>(0, glowAddress + 0x25);
            M.Write<byte>((byte)type, glowAddress + 0x26);
        }

        public Vector3 BonePosition(int boneId)
        {
            IntPtr BoneMatrix = this.BoneMatrix;

            float x = M.Read<float>(BoneMatrix + 0x30 * boneId + 0xC);
            float y = M.Read<float>(BoneMatrix + 0x30 * boneId + 0x1C);
            float z = M.Read<float>(BoneMatrix + 0x30 * boneId + 0x2C);

            return new Vector3(x, y, z);
        }
        public Vector3 BonePosition(Bone boneId) => this.BonePosition((int)boneId);

        public Vector3 BoneAngles(int iBoneId, Vector3 eyePos, Vector3 viewAng)
        {
            Vector3 posTarget = this.BonePosition(iBoneId) - eyePos;
            
            Vector3 output = posTarget.ToEulerAngles() - viewAng;

            return output.Safe();
        }
        public Vector3 BoneAngles(Bone bone, Vector3 eyePos, Vector3 viewAng) => BoneAngles((int)bone, eyePos, viewAng);

        public Vector3 AimPunch
        {
            get
            {
                return M.Read<Vector3>(this.entPointer + Offsets.m_vecPunch);
            }
        }
        public Vector3 Velocity
        {
            get
            {
                return M.Read<Vector3>(this.entPointer + Offsets.m_vecVelocity);
            }
        }
        public Vector3 EyePosition
        {
            get
            {
                return this.Position + M.Read<Vector3>(this.entPointer + Offsets.m_vecViewOffset);
            }
        }
        public Vector3 Position
        {
            get
            {
                return M.Read<Vector3>(this.entPointer + Offsets.m_vecOrigin);
            }
        }
        public Vector3 VecMax
        {
            get
            {
                return M.Read<Vector3>(this.entPointer + Offsets.m_vecMaxs);
            }
        }
    }
}
