using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZodiacGlass
{
    internal class ZodiacGlassObserver : IDisposable
    {
        private readonly IVirtualZodiacGlass glass;

        private readonly Timer timer;

        private int equippedMainHandLightAmount;

        private int equippedOffHandLightAmount;

        private int equippedMainHandID;

        private int equippedOffHandID;

        public ZodiacGlassObserver(IVirtualZodiacGlass glass)
        {
            this.glass = glass;

            this.equippedMainHandLightAmount = glass.GetEquippedMainHandLightAmount();
            this.equippedOffHandLightAmount = glass.GetEquippedOffHandLightAmount();
            this.equippedMainHandID = glass.GetEquippedMainHandID();
            this.equippedOffHandID = glass.GetEquippedOffHandID();

            this.timer = new Timer(this.OnTimerElapsed, null, 500, 500);
        }

        public event EventHandler<ValueChangedEventArgs<int>> EquippedMainHandLightAmountChanged;

        public event EventHandler<ValueChangedEventArgs<int>> EquippedOffHandLightAmountChanged;

        public event EventHandler<ValueChangedEventArgs<int>> EquippedMainHandIDChanged;

        public event EventHandler<ValueChangedEventArgs<int>> EquippedOffHandIDChanged;

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
            this.EquippedMainHandID = glass.GetEquippedMainHandID();
            this.EquippedOffHandID = glass.GetEquippedOffHandID();
            this.EquippedMainHandLightAmount = glass.GetEquippedMainHandLightAmount();
            this.EquippedOffHandLightAmount = glass.GetEquippedOffHandLightAmount();
        }

        public void Dispose()
        {
            this.timer.Dispose();
        }
    }
}
