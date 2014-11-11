namespace ZodiacGlass.FFXIV
{
    using System;
    using System.Linq;

    internal class FFXIVMemoryMap
    {
        private static readonly Lazy<FFXIVMemoryMap> _default = new Lazy<FFXIVMemoryMap>(() => new FFXIVMemoryMap() { InventoryAddress = new int[] { 0x103B320, 0 }, EquippedMainHandOffset = 0x1988, EquippedOffHandOffset = 0x19C8, SpiritBondOffset = 0x8 });

        public static FFXIVMemoryMap Default
        {
            get
            {
                return _default.Value;
            }
        }

        public int EquippedMainHandOffset { get; set; }

        public int EquippedOffHandOffset { get; set; }

        public int SpiritBondOffset { get; set; }

        public int[] InventoryAddress { get; set; }

        public override int GetHashCode()
        {
            return this.EquippedMainHandOffset.GetHashCode() + this.EquippedOffHandOffset.GetHashCode() + this.SpiritBondOffset.GetHashCode() + (this.InventoryAddress ?? new int[0]).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            FFXIVMemoryMap other = obj as FFXIVMemoryMap;
            return other != null && this.EquippedMainHandOffset == other.EquippedMainHandOffset && this.EquippedOffHandOffset == other.EquippedOffHandOffset && this.SpiritBondOffset == other.SpiritBondOffset && Enumerable.SequenceEqual(this.InventoryAddress, other.InventoryAddress);
        }
    }
}
