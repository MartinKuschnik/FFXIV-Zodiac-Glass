using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZodiacGlass.FFXIV
{
    public struct FFXIVPointer
    {
        public int BaseAddressOffset { get; set; }

        public int[] Offsets { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is FFXIVPointer)
            {
                FFXIVPointer other = (FFXIVPointer)obj;

                return this.BaseAddressOffset == other.BaseAddressOffset && Enumerable.SequenceEqual(this.Offsets ?? new int[0], other.Offsets ?? new int[0]);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.BaseAddressOffset.GetHashCode() + (this.Offsets ?? new int[0]).Sum(x => x).GetHashCode();
        }
    }
}
