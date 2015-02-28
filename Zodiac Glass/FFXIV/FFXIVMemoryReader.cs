namespace ZodiacGlass.FFXIV
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using ZodiacGlass.FFXIV;
    using ZodiacGlass.Native;

    internal unsafe class FFXIVMemoryReader : IDisposable
    {
        #region Fields

        private readonly Process process;

        private readonly IntPtr processHandel;

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
            }
        }
        
        #endregion

        #region Properties


        public Process Process
        {
            get { return this.process; }
        }
        

        #endregion

        #region Functions

        public FFXIVItemSet ReadItemSet()
        {
            unsafe
            {
                if (!this.process.HasExited)
                {
                    int* p = (int*)(this.process.MainModule.BaseAddress + this.memMep.ItemSetPointer.BaseAddressOffset);

                    foreach (int offset in this.memMep.ItemSetPointer.Offsets)
                        p = (int*)(this.Read<int>(p) + offset);

                    return this.Read<FFXIVItemSet>(p);

                }
                else
                {
                    return default(FFXIVItemSet);
                }
            }
        }

        private unsafe T Read<T>(void* p) where T : struct
        {
            byte[] buffer = new byte[Marshal.SizeOf(default(T))];
            int readCount;

            NativeMethods.TryReadProcessMemory(this.processHandel, (IntPtr)p, ref buffer, buffer.Length, out readCount);

            fixed (byte* pBuffer = &buffer[0])
            {
                return (T)Marshal.PtrToStructure((IntPtr)pBuffer, typeof(T));
            }
        }

        public void Dispose()
        {
            NativeMethods.CloseHandle(this.processHandel);
        }

        #endregion
    }
}
