using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Control
{
    public static class Controls
    {
        public static void Jump() => M.Write<byte>(6, G.ClientBase + Offsets.m_dwForceJump);
        public static void Attack()
        {
            M.Write<byte>(1, G.ClientBase + Offsets.m_dwForceAttack);
            Thread.Sleep(50);
            M.Write<byte>(0, G.ClientBase + Offsets.m_dwForceAttack);
        }

        public static bool Left
        {
            get
            {
                return M.Read<byte>(G.ClientBase + Offsets.m_dwForceLeft) == 1;
            }
            set
            {
                M.Write(value ? (byte)1 : (byte)0, G.ClientBase + Offsets.m_dwForceLeft);
            }
        }
        public static bool Right
        {
            get
            {
                return M.Read<byte>(G.ClientBase + Offsets.m_dwForceRight) == 1;
            }
            set
            {
                M.Write(value ? (byte)1 : (byte)0, G.ClientBase + Offsets.m_dwForceRight);
            }
        }
    }
}
