using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Control
{
    public static class M
    {
        public static T Read<T>(IntPtr Address) where T : struct => H.Memory.Read<T>(Address);
        public static void Write<T>(T value, IntPtr Address) where T : struct => H.Memory.Write(value, Address);
        public static void Write(byte[] value, IntPtr Address) => H.Memory.WriteMemory(value, Address);
        public static Matrix ReadMatrix(IntPtr address, int rows, int columns)
        {
            Matrix matrix = new Matrix(rows, columns);
            byte[] data = H.Memory.Read(address, sizeof(float) * rows * columns);
            matrix.Read(data);

            return matrix;
        }
        public static string ReadString(IntPtr address) => H.Memory.ReadString(address);
        public static void WriteString(string value, IntPtr address) => H.Memory.WriteMemory(Encoding.Default.GetBytes(value), address);
    }

    public class Memory
    {
        private Process TargetProcess { get; set; }
        private IntPtr Handle { get; set; }

        /// <summary>
        /// Initialize a new memory class
        /// </summary>
        /// <param name="process">Target Process</param>
        public Memory(Process process)
        {
            TargetProcess = process;
            Handle = process.Handle; 
        }

        /// <summary>
        /// Get module handle
        /// </summary>
        /// <param name="ModuleName">Module Name</param>
        /// <returns></returns>
        public ProcessModule GetModule(string ModuleName) => 
            TargetProcess.Modules.Cast<ProcessModule>().FirstOrDefault(s => s.ModuleName.Equals(ModuleName, StringComparison.OrdinalIgnoreCase));

        public static T GetStructure<T>(byte[] bytes)
        {
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            var structure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return structure;
        }
        public static T GetStructure<T>(byte[] bytes, int index)
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] tmp = new byte[size];
            Array.Copy(bytes, index, tmp, 0, size);
            return GetStructure<T>(tmp);
        }

        /// <summary>
        /// Read process memory
        /// </summary>
        /// <typeparam name="T">Data Type</typeparam>
        /// <param name="Address">Memory Address</param>
        /// <returns></returns>
        public T Read<T>(IntPtr Address)
        {
            var size = Marshal.SizeOf(typeof(T));
            var data = Read(Address, size);
            return GetStructure<T>(data);
        }

        /// <summary>
        /// Write Process Memory
        /// </summary>
        /// <typeparam name="T">Data Type</typeparam>
        /// <param name="input">Data</param>
        /// <param name="Address">Memory Address</param>
        public void Write<T>(T input, IntPtr Address)
        {
            int size = Marshal.SizeOf(input);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(input, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            WriteMemory(arr, Address);
        }

        /// <summary>
        /// Read a chunk from memory
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="data">Data</param>
        /// <param name="length">Length of chunk</param>
        public byte[] Read(IntPtr address, int length)
        {
            byte[] tempData = new byte[length];
            Windows.ReadProcessMemory(Handle, address, tempData, length, 0);
            return tempData;
        }

        /// <summary>
        /// Read a char[255] from memory
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>String from memory</returns>
        public string ReadString(IntPtr address)
        {
            byte[] numArray = Read(address, 255);
            var str = Encoding.Default.GetString(numArray);

            if (str.Contains('\0'))
                str = str.Substring(0, str.IndexOf('\0'));
            return str;
        }

        public void WriteMemory(byte[] bytes, IntPtr address) => Windows.WriteProcessMemory(Handle, address, bytes, bytes.Length, 0);
    }
}
