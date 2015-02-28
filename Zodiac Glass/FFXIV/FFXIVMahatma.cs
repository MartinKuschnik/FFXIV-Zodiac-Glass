namespace ZodiacGlass.FFXIV
{
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    [DebuggerDisplay("{Charge} - {ChargePercentage} %")]
    [StructLayout(LayoutKind.Explicit, Size = FFXIVStructSizes.Weapon)]
    internal struct FFXIVMahatma
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static readonly FFXIVMahatma None = default(FFXIVMahatma);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static readonly FFXIVMahatma Empty = (FFXIVMahatma)1;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static readonly FFXIVMahatma Full = (FFXIVMahatma)80;
        
        [FieldOffset(0)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly short value;

        private FFXIVMahatma(short value)
        {
            this.value = value;
        }
        
        public short Charge
        {
            get 
            {
                if (this == None)
                {
                    return None;
                }
                else
                {
                    return (short)(this - Empty); 
                }

            }
        }

        public float ChargePercentage
        {
            get
            {
                return 100f * (float)this.Charge / (FFXIVMahatma.Full - FFXIVMahatma.Empty);
            }
        }

        public static implicit operator FFXIVMahatma(short value)
        {
            return new FFXIVMahatma(value);
        }

        public static implicit operator short(FFXIVMahatma mahatma)
        {
            return mahatma.value;
        }
    }
}
