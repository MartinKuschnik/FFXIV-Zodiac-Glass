namespace ZodiacGlass
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using ZodiacGlass.FFXIV;

    internal class OverlayViewModel : ViewModelBase
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const int AdditionLifeTime = 20000;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const int MaxLight = 2000;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private FFXIVMemoryReader glass;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private OverlayDisplayMode mode;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private FFXIVMemoryObserver observer;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int ignoreNextMainHandAdditionCount;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool ignoreNextOffHandAddition;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int mainHandAddition;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int offHandAddition;

        public OverlayViewModel()
        {


        }

        public FFXIVMemoryReader MemoryReader
        {
            get
            {
                return this.glass;
            }
            set
            {
                if (this.observer != null)
                {
                    this.observer.EquippedMainHandLightAmountChanged -= this.OnEquippedMainHandLightAmountChanged;
                    this.observer.EquippedOffHandLightAmountChanged -= this.OnEquippedOffHandLightAmountChanged;
                    this.observer.EquippedMainHandIDChanged -= this.OnEquippedMainHandIDChanged;
                    this.observer.EquippedOffHandIDChanged -= this.OnEquippedOffHandIDChanged;
                    this.observer.CurrentMahatmaChangeChanged -= this.OnCurrentMahatmaChangeChanged;
                    this.observer.CurrentMahatmaChanged -= this.OnCurrentMahatmaChanged;
                    this.observer.Dispose();
                    this.observer = null;
                }

                this.glass = value;

                if (this.glass != null)
                {
                    this.observer = new FFXIVMemoryObserver(this.glass);

                    this.observer.EquippedMainHandLightAmountChanged += this.OnEquippedMainHandLightAmountChanged;
                    this.observer.EquippedOffHandLightAmountChanged += this.OnEquippedOffHandLightAmountChanged;
                    this.observer.EquippedMainHandIDChanged += this.OnEquippedMainHandIDChanged;
                    this.observer.EquippedOffHandIDChanged += this.OnEquippedOffHandIDChanged;
                    this.observer.CurrentMahatmaChangeChanged += this.OnCurrentMahatmaChangeChanged;
                    this.observer.CurrentMahatmaChanged += this.OnCurrentMahatmaChanged;
                }

                this.NotifyPropertyChanged(() => this.IsOverlayVisible);
                this.NotifyPropertyChanged(() => this.ClassSymbolUri);
                this.NotifyPropertyChanged(() => this.EquippedMainHandLightAmount);
                this.NotifyPropertyChanged(() => this.EquippedOffHandLightAmount);
                this.NotifyPropertyChanged(() => this.IsSeparatorVisible);
                this.NotifyPropertyChanged(() => this.IsMainHandVisible);
                this.NotifyPropertyChanged(() => this.IsOffHandVisible);
            }
        }

        private void OnCurrentMahatmaChanged(object sender, ValueChangedEventArgs<FFXIVMahatma> e)
        {
            // necessary for switch from FFXIVMahatma.Full to FFXIVMahatma.None and FFXIVMahatma.Empty because the charge will not change
            this.NotifyPropertyChanged(() => this.EquippedMainHandLightAmount);
        }

        private void OnCurrentMahatmaChangeChanged(object sender, ValueChangedEventArgs<int> e)
        {
            this.NotifyPropertyChanged(() => this.EquippedMainHandLightAmount);

            if (this.ignoreNextMainHandAdditionCount-- <= 0)
            {
                if (this.glass != null)
                {
                    FFXIVItemSet itemSet = this.glass.ReadItemSet();

                    if (Enum.IsDefined(typeof(FFXIVZodiacWeaponID), itemSet.Weapon.ID) && e.NewValue > e.OldValue)
                    {
                        this.MainHandAddition = e.NewValue - e.OldValue;

                        Task.Factory.StartNew(() =>
                        {
                            Thread.Sleep(AdditionLifeTime);
                            this.MainHandAddition = 0;
                        });
                    }
                }
            }
        }

        private void OnEquippedMainHandLightAmountChanged(object sender, ValueChangedEventArgs<int> e)
        {
            this.NotifyPropertyChanged(() => this.EquippedMainHandLightAmount);

            if (this.ignoreNextMainHandAdditionCount-- == 0)
            {
                if (this.glass != null)
                {
                    FFXIVItemSet itemSet = this.glass.ReadItemSet();

                    if (Enum.IsDefined(typeof(FFXIVNovusWeaponID), itemSet.Weapon.ID))
                    {
                        this.MainHandAddition = e.NewValue - e.OldValue;

                        Task.Factory.StartNew(() =>
                        {
                            Thread.Sleep(AdditionLifeTime);
                            this.MainHandAddition = 0;
                        });
                    }

                }
            }
        }

        private void OnEquippedOffHandLightAmountChanged(object sender, ValueChangedEventArgs<int> e)
        {
            this.NotifyPropertyChanged(() => this.EquippedOffHandLightAmount);

            if (!this.ignoreNextOffHandAddition)
            {
                this.OffHandAddition = e.NewValue - e.OldValue;

                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(AdditionLifeTime);
                    this.OffHandAddition = 0;
                });
            }
            else
            {
                this.ignoreNextOffHandAddition = false;
            }
        }

        private void OnEquippedMainHandIDChanged(object sender, ValueChangedEventArgs<int> e)
        {
            this.NotifyPropertyChanged(() => this.IsOverlayVisible);
            this.NotifyPropertyChanged(() => this.IsMainHandVisible);
            this.NotifyPropertyChanged(() => this.IsSeparatorVisible);
            this.NotifyPropertyChanged(() => this.ClassSymbolUri);

            FFXIVItemSet itemSet = this.glass.ReadItemSet();

            if (itemSet.Weapon.IsNovusWeapon || itemSet.Shield.IsNovusWeapon)
            {
                this.ignoreNextMainHandAdditionCount = 1;
            }
            else if (itemSet.Weapon.IsZodiacWeapon)
            {
                this.ignoreNextMainHandAdditionCount = 2;
            }
        }

        private void OnEquippedOffHandIDChanged(object sender, ValueChangedEventArgs<int> e)
        {
            this.NotifyPropertyChanged(() => this.IsOverlayVisible);
            this.NotifyPropertyChanged(() => this.IsOffHandVisible);
            this.NotifyPropertyChanged(() => this.IsSeparatorVisible);
            this.NotifyPropertyChanged(() => this.ClassSymbolUri);
            this.ignoreNextOffHandAddition = true;
        }

        public object ClassSymbolUri
        {
            get
            {
                if (this.glass != null)
                {
                    string className = null;

                    FFXIVItemSet itemSet = this.glass.ReadItemSet();

                    if (itemSet.Weapon.ID == (int)FFXIVNovusWeaponID.CurtanaNovus
                        || itemSet.Weapon.ID == (int)FFXIVZodiacWeaponID.Exkalibur
                        || itemSet.Shield.ID == (int)FFXIVNovusWeaponID.HolyShieldNovus
                        || itemSet.Shield.ID == (int)FFXIVZodiacWeaponID.AegisShield)
                    {
                        className = "paladin";
                    }
                    else if (itemSet.Weapon.ID == (int)FFXIVNovusWeaponID.SphairaiNovus
                      || itemSet.Weapon.ID == (int)FFXIVZodiacWeaponID.KaiserKnuckles)
                    {
                        className = "monk";
                    }
                    else if (itemSet.Weapon.ID == (int)FFXIVNovusWeaponID.BravuraNovus
                      || itemSet.Weapon.ID == (int)FFXIVZodiacWeaponID.Ragnarok)
                    {
                        className = "warrior";
                    }
                    else if (itemSet.Weapon.ID == (int)FFXIVNovusWeaponID.GaeBolgNovus
                      || itemSet.Weapon.ID == (int)FFXIVZodiacWeaponID.Longinus)
                    {
                        className = "dragoon";
                    }
                    else if (itemSet.Weapon.ID == (int)FFXIVNovusWeaponID.ArtemisBowNovus
                      || itemSet.Weapon.ID == (int)FFXIVZodiacWeaponID.YoichiBow)
                    {
                        className = "bard";
                    }
                    else if (itemSet.Weapon.ID == (int)FFXIVNovusWeaponID.ThyrusNovus
                      || itemSet.Weapon.ID == (int)FFXIVZodiacWeaponID.Nirvana)
                    {
                        className = "whitemage";
                    }
                    else if (itemSet.Weapon.ID == (int)FFXIVNovusWeaponID.StardustRodNovus
                      || itemSet.Weapon.ID == (int)FFXIVZodiacWeaponID.LilithRod)
                    {
                        className = "blackmage";
                    }
                    else if (itemSet.Weapon.ID == (int)FFXIVNovusWeaponID.TheVeilofWiyuNovus
                      || itemSet.Weapon.ID == (int)FFXIVZodiacWeaponID.Apocalypse)
                    {
                        className = "summoner";
                    }
                    else if (itemSet.Weapon.ID == (int)FFXIVNovusWeaponID.OmnilexNovus
                      || itemSet.Weapon.ID == (int)FFXIVZodiacWeaponID.LastResort)
                    {
                        className = "scholar";
                    }
                    else if (itemSet.Weapon.ID == (int)FFXIVNovusWeaponID.YoshimitsuNovus
                      || itemSet.Weapon.ID == (int)FFXIVZodiacWeaponID.SasukesBlades)
                    {
                        className = "ninja";
                    }

                    if (className != null)
                        return new Uri(string.Format("pack://application:,,,/Zodiac Glass;component/Resources/classimages/{0}.png", className));
                }

                return DependencyProperty.UnsetValue;
            }

        }

        public string EquippedMainHandLightAmount
        {
            get
            {
                if (this.glass != null)
                {
                    FFXIVItemSet itemSet = this.glass.ReadItemSet();

                    if (Enum.IsDefined(typeof(FFXIVNovusWeaponID), itemSet.Weapon.ID))
                    {
                        switch (this.Mode)
                        {
                            case OverlayDisplayMode.Normal:
                                return itemSet.Weapon.LightAmount.ToString();
                            case OverlayDisplayMode.Percentage:
                                return string.Format("{0} %", Math.Round(100 * (float)itemSet.Weapon.LightAmount / MaxLight, 2));
                            default:
                                return null;
                        }
                    }
                    else if (Enum.IsDefined(typeof(FFXIVZodiacWeaponID), itemSet.Weapon.ID))
                    {
                        FFXIVWeapon weapon = itemSet.Weapon;
                        int curMahatmas = weapon.Mahatmas.Count(m => m != FFXIVMahatma.None);

                        switch (this.Mode)
                        {
                            case OverlayDisplayMode.Normal:
                                return string.Format("{0}/{1} - {2}/{3}", curMahatmas, weapon.Mahatmas.Length, weapon.CurrentMahatma.Charge, FFXIVMahatma.Full.Charge);
                            case OverlayDisplayMode.Percentage:
                                return string.Format("{0}/{1} - {2} %", curMahatmas, weapon.Mahatmas.Length, Math.Round(weapon.CurrentMahatma.ChargePercentage, 2));
                            default:
                                return null;
                        }
                    }
                }

                return null;
            }
        }

        public string EquippedOffHandLightAmount
        {
            get
            {
                int val = 0;

                if (this.glass != null)
                {
                    FFXIVItemSet itemSet = this.glass.ReadItemSet();
                    val = itemSet.Shield.LightAmount;
                }

                return this.Mode == OverlayDisplayMode.Normal ? val.ToString() : string.Format("{0} %", Math.Round(100 * (float)val / MaxLight, 2));
            }
        }

        public OverlayDisplayMode Mode
        {
            get
            {
                return mode;
            }
            set
            {
                if (mode != value)
                {
                    mode = value;
                    this.NotifyPropertyChanged(() => this.Mode);
                    this.NotifyPropertyChanged(() => this.EquippedMainHandLightAmount);
                    this.NotifyPropertyChanged(() => this.EquippedOffHandLightAmount);
                }
            }
        }

        public int MainHandAddition
        {
            get
            {
                return this.mainHandAddition;
            }
            set
            {
                if (this.mainHandAddition != value)
                {
                    this.mainHandAddition = value;
                    this.NotifyPropertyChanged(() => this.MainHandAddition);
                    this.NotifyPropertyChanged(() => this.IsMainHandAdditionVisible);
                }
            }
        }

        public int OffHandAddition
        {
            get
            {
                return this.offHandAddition;
            }
            set
            {
                if (this.offHandAddition != value)
                {
                    this.offHandAddition = value;
                    this.NotifyPropertyChanged(() => this.OffHandAddition);
                    this.NotifyPropertyChanged(() => this.IsOffHandAdditionVisible);
                }
            }
        }
        public bool IsMainHandAdditionVisible
        {
            get
            {
                return this.IsMainHandVisible && this.mainHandAddition > 0;
            }
        }

        public bool IsOffHandAdditionVisible
        {
            get
            {
                return this.IsOffHandVisible && this.offHandAddition > 0;
            }
        }

        public bool IsSeparatorVisible
        {
            get
            {
                return this.IsMainHandVisible && this.IsOffHandVisible;
            }
        }

        public bool IsMainHandVisible
        {
            get
            {
                if (this.glass != null)
                {
                    FFXIVWeapon weapon = this.glass.ReadItemSet().Weapon;

                    return weapon.IsNovusWeapon || weapon.IsZodiacWeapon;
                }

                return false;
            }
        }

        public bool IsOffHandVisible
        {
            get
            {
                if (this.glass != null)
                {
                    FFXIVWeapon weapon = this.glass.ReadItemSet().Weapon;

                    return weapon.IsNovusWeapon; // we only support IsOffHandVisible for the novus shield
                }

                return false;
            }
        }

        public bool IsOverlayVisible
        {
            get
            {
                if (this.glass != null)
                {
                    FFXIVItemSet itemset = this.glass.ReadItemSet();

                    return itemset.Weapon.IsNovusWeapon || itemset.Weapon.IsZodiacWeapon || itemset.Shield.IsNovusWeapon;
                }

                return false;
            }
        }
    }
}
