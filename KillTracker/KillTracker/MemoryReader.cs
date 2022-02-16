using System;
using System.Runtime.InteropServices;

namespace KillTracker
{
    class MemoryReader
    {
        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        private int[] SCREEN_STATE = { 0x15446C, 0x58 };
        private int[] KILLS = { 0x154510, 0x30, 0x280, 0x90 };

        private int processHandle;
        private int baseAddress;

        public MemoryReader(int processHandle, int baseAddress)
        {
            this.processHandle = processHandle;
            this.baseAddress = baseAddress;
        }

        public int ReadKills()
        {
            byte[] buffer = new byte[1];
            ReadMemory(buffer, baseAddress, KILLS);
            return buffer[0];
        }

        public int ReadScreenState()
        {
            byte[] buffer = new byte[1];
            ReadMemory(buffer, baseAddress, SCREEN_STATE);
            return buffer[0];
        }

        private byte[] ReadMemory(byte[] buffer, int addr, int[] offsets)
        {
            int bytesRead = 0;

            // Buffer for next pointer
            byte[] pointer = new byte[4];

            // Traverse pointer path
            for (int i = 0; i < offsets.Length - 1; i++)
            {
                addr += offsets[i];
                ReadProcessMemory(processHandle, addr, pointer, pointer.Length, ref bytesRead);
                addr = BitConverter.ToInt32(pointer, 0);
            }

            // Read value from final address
            addr += offsets[offsets.Length - 1];
            ReadProcessMemory(processHandle, addr, buffer, buffer.Length, ref bytesRead);

            return buffer;
        }
    }
}