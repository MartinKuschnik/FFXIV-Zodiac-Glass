namespace ZodiacGlass.FFXIV
{
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Size = 64)]
    internal unsafe struct FFXIVItem
    {
        [FieldOffset(0)]
        private fixed byte raw[64];

        [FieldOffset(0)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly int id;


        public int ID
        {
            get
            {
                return this.id;
            }
        }

    }
}
