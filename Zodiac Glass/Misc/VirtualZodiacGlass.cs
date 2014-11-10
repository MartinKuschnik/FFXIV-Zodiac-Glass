namespace ZodiacGlass
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    internal class VirtualZodiacGlass : IVirtualZodiacGlass, IDisposable
    {
        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Process process;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IntPtr processHandel;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IntPtr equippedMainHandAddress;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IntPtr equippedOffHandAddress;

        private readonly MemoryMap memMep;

        #endregion

        #region Constructors

        public VirtualZodiacGlass(Process process, MemoryMap memMep)
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

        #region Functions

        private IntPtr CalculateAddress(int offset)
        {
            IntPtr addr = this.process.MainModule.BaseAddress;

            foreach (IntPtr addrPointer in this.memMep.InventoryAddress)
                addr = (IntPtr)this.ReadInt32(IntPtr.Add(addr, (int)addrPointer));

            return IntPtr.Add(addr, offset);
        }

        private short ReadInt16(IntPtr addr)
        {
            byte[] array = new byte[3];
            int num = 1;
            NativeMethods.ReadProcessMemory(this.processHandel, addr, array, 2, ref num);
            return BitConverter.ToInt16(array, 0);
        }

        private int ReadInt32(IntPtr addr)
        {
            byte[] array = new byte[5];
            int num = 1;
            NativeMethods.ReadProcessMemory(this.processHandel, addr, array, 4, ref num);
            return BitConverter.ToInt32(array, 0);
        }

        public void Dispose()
        {
            NativeMethods.CloseHandle(this.processHandel);
        }

        #endregion
    }
}
