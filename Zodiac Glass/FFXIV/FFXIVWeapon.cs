namespace ZodiacGlass.FFXIV
{
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Size=64)]
    internal unsafe struct FFXIVWeapon
    {

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static readonly FFXIVWeapon None = default(FFXIVWeapon);

        [FieldOffset(0)]
        private fixed byte raw[64];

        [FieldOffset(0)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly int id;

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

        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is FFXIVWeapon && ((FFXIVWeapon)obj).id == this.id;
        }
        
        public static bool operator ==(FFXIVWeapon a, FFXIVWeapon b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(FFXIVWeapon a, FFXIVWeapon b)
        {
            return !a.Equals(b);
        }
    }
}
