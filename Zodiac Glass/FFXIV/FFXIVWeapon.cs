namespace ZodiacGlass.FFXIV
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Size = FFXIVStructSizes.Weapon)]
    internal unsafe struct FFXIVWeapon
    {
        // because each 500 light begins a new Mahatma
        // 0   = 0 stones
        // 1   = 1 stone and 0 light
        // 80  = fist stone full
        // 501 = 1 stone full and one empty one
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const int MahatmaPadding = 420;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static readonly FFXIVWeapon None = default(FFXIVWeapon);
        
        [FieldOffset(0)]
        private fixed byte raw[FFXIVStructSizes.Weapon];

        [FieldOffset(0)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly int id;

        [FieldOffset(sizeof(long))]
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

        public FFXIVMahatma CurrentMahatma
        {
            get
            {
                return (FFXIVMahatma)(short)(this.lightAmount % (MahatmaPadding + FFXIVMahatma.Full));
            }
        }

        public FFXIVMahatma[] Mahatmas
        {
            get
            {                

                FFXIVMahatma[] value = new FFXIVMahatma[12];

                if (this.lightAmount > 0)
                {
                    int fullStones = (this.lightAmount / (MahatmaPadding + FFXIVMahatma.Full));

                    int i;

                    for (i = 0; i < fullStones; i++)
                    {
                        value[i] = FFXIVMahatma.Full;
                    }

                    if (i < value.Length)
                    {
                        value[i] = this.CurrentMahatma;
                    }
                }

                return value;
            }
        }

        public bool IsZodiacWeapon
        {
            get 
            {
                return Enum.IsDefined(typeof(FFXIVZodiacWeaponID), this.id);
            }
        }

        public bool IsNovusWeapon
        {
            get
            {
                return Enum.IsDefined(typeof(FFXIVNovusWeaponID), this.id);
            }
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is FFXIVWeapon && ((FFXIVWeapon)obj).id == this.id;
        }

        public static bool operator ==(FFXIVWeapon c1, FFXIVWeapon c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(FFXIVWeapon c1, FFXIVWeapon c2)
        {
            return !c1.Equals(c2);
        }
        
    }
}
