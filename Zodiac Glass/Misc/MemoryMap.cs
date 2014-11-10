namespace ZodiacGlass
{
    using System;
    using System.Linq;

    public class MemoryMap
    {
        private static readonly Lazy<MemoryMap> _default = new Lazy<MemoryMap>(() => new MemoryMap() { InventoryAddress = new int[] { 0x103B320, 0 }, EquippedMainHandOffset = 0x1988, EquippedOffHandOffset = 0x19C8, SpiritBondOffset = 0x8 });

        public static MemoryMap Default
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
            MemoryMap other = obj as MemoryMap;
            return other != null && this.EquippedMainHandOffset == other.EquippedMainHandOffset && this.EquippedOffHandOffset == other.EquippedOffHandOffset && this.SpiritBondOffset == other.SpiritBondOffset && Enumerable.SequenceEqual(this.InventoryAddress, other.InventoryAddress);
        }
    }
}
