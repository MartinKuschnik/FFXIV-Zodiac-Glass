namespace ZodiacGlass.FFXIV
{
    using System;
    using System.Linq;

    public class FFXIVMemoryMapV2
    {
        private static readonly Lazy<FFXIVMemoryMapV2> _default = new Lazy<FFXIVMemoryMapV2>(() => new FFXIVMemoryMapV2() { InventoryAddress = 0x103B320, EquippedMainHandOffset = 0x1988, EquippedOffHandOffset = 0x19C8});

        public static FFXIVMemoryMapV2 Default
        {
            get
            {
                return _default.Value;
            }
        }

        public int EquippedMainHandOffset { get; set; }

        public int EquippedOffHandOffset { get; set; }

        public int InventoryAddress { get; set; }

        public override int GetHashCode()
        {
            return this.EquippedMainHandOffset.GetHashCode() + this.EquippedOffHandOffset.GetHashCode() +  this.InventoryAddress.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            FFXIVMemoryMapV2 other = obj as FFXIVMemoryMapV2;
            return other != null && this.EquippedMainHandOffset == other.EquippedMainHandOffset && this.EquippedOffHandOffset == other.EquippedOffHandOffset && this.InventoryAddress == other.InventoryAddress;
        }
    }
}
