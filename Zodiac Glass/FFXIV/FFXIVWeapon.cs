namespace ZodiacGlass.FFXIV
{
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Size = 10)]
    internal unsafe struct FFXIVWeapon
    {
        [FieldOffset(0)]
        private fixed byte raw[10];

        [FieldOffset(0)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly int id;

        [FieldOffset(4)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private fixed byte byte4to7[4];

        [FieldOffset(8)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly short lightAmount;


        public int ID
        {
            get
            {
                return this.id;
            }
        }

        public short LightAmount
        {
            get
            {
                return this.lightAmount;
            }
        }

    }
}
