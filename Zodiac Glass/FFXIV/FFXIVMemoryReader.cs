namespace ZodiacGlass.FFXIV
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using ZodiacGlass.FFXIV;
    using ZodiacGlass.Native;

    internal unsafe class FFXIVMemoryReader :  IDisposable
    {
        #region Fields

        private readonly Process process;

        private readonly IntPtr processHandel;

        private readonly IntPtr equippedMainHandAddress;

        private readonly FFXIVWeapon* mainHand;

        private readonly IntPtr equippedOffHandAddress;

        private readonly FFXIVMemoryMap memMep;

        #endregion

        #region Constructors

        public FFXIVMemoryReader(Process process, FFXIVMemoryMap memMep)
        {
            if (process == null)
                throw new ArgumentNullException(MethodBase.GetCurrentMethod().GetParameters()[0].Name);

            if (memMep == null)
                throw new ArgumentNullException(MethodBase.GetCurrentMethod().GetParameters()[1].Name);

            this.process = process;
            this.memMep = memMep;

            if (!this.process.HasExited)
            {
                this.processHandel = NativeMethods.OpenProcess(ProcessAccessFlags.VirtualMemoryRead | ProcessAccessFlags.QueryInformation, false, this.process.Id);
                
                this.equippedMainHandAddress = this.CalculateAddress(this.memMep.EquippedMainHandOffset);
                this.equippedOffHandAddress = this.CalculateAddress(this.memMep.EquippedOffHandOffset);
            }
        }

        #endregion

        #region Functions


        public int GetEquippedMainHandLightAmount()
        {
            return this.ReadInt16(IntPtr.Add(this.equippedMainHandAddress, this.memMep.SpiritBondOffset));
        }

        public int GetEquippedOffHandLightAmount()
        {
            return this.ReadInt16(IntPtr.Add(this.equippedOffHandAddress, this.memMep.SpiritBondOffset));
        }

        public int GetEquippedMainHandID()
        {
            return this.ReadInt32((IntPtr)this.equippedMainHandAddress);
        }

        public int GetEquippedOffHandID()
        {
            return this.ReadInt32((IntPtr)this.equippedOffHandAddress);
        }

        private unsafe IntPtr CalculateAddress(int offset)
        {
            IntPtr addr = this.process.MainModule.BaseAddress;

            foreach (IntPtr addrPointer in this.memMep.InventoryAddress)
                addr = (IntPtr)this.ReadInt32(IntPtr.Add(addr, (int)addrPointer));

            return IntPtr.Add(addr, offset);
        }

        private short ReadInt16(IntPtr addr)
        {
            byte[] buffer = new byte[sizeof(short)];
            int readCount = 0;

            NativeMethods.ReadProcessMemory(this.processHandel, addr, buffer, sizeof(short), ref readCount);

            fixed (byte* p = &buffer[0])
            {
                return *(short*)p;
            }
        }

        private int ReadInt32(IntPtr addr)
        {
            byte[] buffer = new byte[sizeof(int)];
            int readCount = 0;

            NativeMethods.ReadProcessMemory(this.processHandel, addr, buffer, sizeof(int), ref readCount);

            fixed (byte* p = &buffer[0])
            {
                return *(int*)p;
            }
        }

        public void Dispose()
        {
            NativeMethods.CloseHandle(this.processHandel);
        }

        #endregion
    }
}
