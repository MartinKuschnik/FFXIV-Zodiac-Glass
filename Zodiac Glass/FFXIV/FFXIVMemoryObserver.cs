namespace ZodiacGlass.FFXIV
{
    using System;
    using System.Threading;

    internal class FFXIVMemoryObserver : IDisposable
    {
        private readonly FFXIVMemoryReader memoryReader;

        private readonly Timer timer;

        private int equippedMainHandLightAmount;

        private int equippedOffHandLightAmount;

        private int equippedMainHandID;

        private int equippedOffHandID;

        public FFXIVMemoryObserver(FFXIVMemoryReader memoryReader)
        {
            this.memoryReader = memoryReader;

            FFXIVItemSet itemSet = memoryReader.ReadItemSet();

            this.equippedMainHandLightAmount = itemSet.Weapon.LightAmount;
            this.equippedOffHandLightAmount = itemSet.Shield.LightAmount;
            this.equippedMainHandID = itemSet.Weapon.ID;
            this.equippedOffHandID = itemSet.Shield.ID;

            this.timer = new Timer(this.OnTimerElapsed, null, 500, 500);
        }

        public event EventHandler<ValueChangedEventArgs<int>> EquippedMainHandLightAmountChanged;

        public event EventHandler<ValueChangedEventArgs<int>> EquippedOffHandLightAmountChanged;

        public event EventHandler<ValueChangedEventArgs<int>> EquippedMainHandIDChanged;

        public event EventHandler<ValueChangedEventArgs<int>> EquippedOffHandIDChanged;

        public FFXIVMemoryReader MemoryReader
        {
            get
            {
                return this.memoryReader;
            }
        }

        private int EquippedMainHandLightAmount
        {
            get
            {
                return this.equippedMainHandLightAmount;
            }
            set
            {
                if (this.equippedMainHandLightAmount != value)
                {
                    int oldValue = this.equippedMainHandLightAmount;

                    this.equippedMainHandLightAmount = value;

                    if (this.EquippedMainHandLightAmountChanged != null)
                        this.EquippedMainHandLightAmountChanged(this, new ValueChangedEventArgs<int>(oldValue, value));
                    
                }

            }
        }

        private int EquippedOffHandLightAmount
        {
            get
            {
                return this.equippedOffHandLightAmount;
            }
            set
            {
                if (this.equippedOffHandLightAmount != value)
                {
                    int oldValue = this.equippedOffHandLightAmount;

                    this.equippedOffHandLightAmount = value;

                    if (this.EquippedOffHandLightAmountChanged != null)
                        this.EquippedOffHandLightAmountChanged(this, new ValueChangedEventArgs<int>(oldValue, value));

                }

            }
        }

        private int EquippedMainHandID
        {
            get
            {
                return this.equippedMainHandID;
            }
            set
            {
                if (this.equippedMainHandID != value)
                {
                    int oldValue = this.equippedMainHandID;

                    this.equippedMainHandID = value;

                    if (this.EquippedMainHandIDChanged != null)
                        this.EquippedMainHandIDChanged(this, new ValueChangedEventArgs<int>(oldValue, value));

                }

            }
        }

        private int EquippedOffHandID
        {
            get
            {
                return this.equippedOffHandID;
            }
            set
            {
                if (this.equippedOffHandID != value)
                {
                    int oldValue = this.equippedOffHandID;

                    this.equippedOffHandID = value;

                    if (this.EquippedOffHandIDChanged != null)
                        this.EquippedOffHandIDChanged(this, new ValueChangedEventArgs<int>(oldValue, value));

                }

            }
        }


        private void OnTimerElapsed(object state)
        {
            if (!this.MemoryReader.Process.HasExited)
            {
                FFXIVItemSet itemSet = memoryReader.ReadItemSet();

                this.EquippedMainHandID = itemSet.Weapon.ID;
                this.EquippedOffHandID = itemSet.Shield.ID;
                this.EquippedMainHandLightAmount = itemSet.Weapon.LightAmount;
                this.EquippedOffHandLightAmount = itemSet.Shield.LightAmount;
            }
            else
            {
                this.timer.Dispose();
            }

        }

        public void Dispose()
        {
            this.timer.Dispose();
        }
    }
}
