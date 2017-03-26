using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Control
{
    public class SigScanner
    {
        IntPtr m_hProcess;
        byte[] m_moduleBuffer;
        int _moduleBaseAddress;

        /// <summary>
        /// Initialize a new SigScanner
        /// </summary>
        /// <param name="hProc"></param>
        public SigScanner(IntPtr hProc)
        {
            m_hProcess = hProc;
        }

        /// <summary>
        /// Dump remote module for pattern scanning
        /// </summary>
        /// <param name="targetModule">Remote Module To Dump</param>
        /// <returns>Module successfully dumped</returns>
        public bool DumpModule(ProcessModule targetModule)
        { 
            int moduleSize = targetModule.ModuleMemorySize;             // Size of Library/Module
            _moduleBaseAddress = targetModule.BaseAddress.ToInt32();    // Base Offset of Library/Module

            int BytesRead = 0;
            m_moduleBuffer = new byte[moduleSize];

            H.ReadProcessMemory(m_hProcess, _moduleBaseAddress, m_moduleBuffer, moduleSize, ref BytesRead);

            return BytesRead > 0;
        }


        /// <summary>
        /// Find Pattern in buffer
        /// </summary>
        /// <param name="nOffset">Start offset</param>
        /// <param name="strPattern">Pattern</param>
        /// <returns>Boolean if pattern was found</returns>
        private bool PatternCheck(int nOffset, string strPattern)
        {
            string[] offsetPatternArray = strPattern.Split(' ');

            for (int x = 0; x < offsetPatternArray.Length; x++)
            {
                if (offsetPatternArray[x] == "?")
                    continue;

                int offsetPatternByte = Convert.ToInt32(offsetPatternArray[x], 16);

                if ((offsetPatternByte != this.m_moduleBuffer[nOffset + x]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Find pattern in dumped module
        /// </summary>
        /// <param name="offsetPattern">Pattern in string format</param>
        /// <param name="additionOffset">Addition to returned offset</param>
        /// <returns>Offsets from pattern</returns>
        public int FindPattern(string pattern, ScanFlags flags, int patternAddition, int addressOffset)
        {
            for (int x = 0; x < m_moduleBuffer.Length; x++)
                if (this.PatternCheck(x, pattern))
                {
                    int address = _moduleBaseAddress + x + patternAddition;

                    if (flags.HasFlag(ScanFlags.READ))
                        address = M.Read<int>((IntPtr)address);

                    if (flags.HasFlag(ScanFlags.SUBSTRACT_BASE))
                        address -= _moduleBaseAddress;

                    return address + addressOffset;
                }

            return 0;
        }
    }
}
