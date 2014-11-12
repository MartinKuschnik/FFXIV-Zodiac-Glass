using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ZodiacGlass.FFXIV;

namespace ZodiacGlass
{
    internal class OverlayViewModel : ViewModelBase
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const int AdditionLifeTime = 20000;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private FFXIVMemoryReader glass;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private OverlayDisplayMode mode;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private FFXIVMemoryObserver observer;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool ignoreNextMainHandAddition;

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
                }


                this.NotifyPropertyChanged(() => this.OverlayVisibility);
                this.NotifyPropertyChanged(() => this.ClassSymbolUri);
                this.NotifyPropertyChanged(() => this.EquippedMainHandLightAmount);
                this.NotifyPropertyChanged(() => this.EquippedOffHandLightAmount);
                this.NotifyPropertyChanged(() => this.SeparatorVisibility);
                this.NotifyPropertyChanged(() => this.MainHandVisibility);
                this.NotifyPropertyChanged(() => this.OffHandVisibility);
            }
        }

        private void OnEquippedMainHandLightAmountChanged(object sender, ValueChangedEventArgs<int> e)
        {
            this.NotifyPropertyChanged(() => this.EquippedMainHandLightAmount);

            if (!this.ignoreNextMainHandAddition)
            {
                this.MainHandAddition = e.NewValue - e.OldValue;

                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(AdditionLifeTime);
                    this.MainHandAddition = 0;
                });
            }
            else
            {
                this.ignoreNextMainHandAddition = false;
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
            this.NotifyPropertyChanged(() => this.OverlayVisibility);
            this.NotifyPropertyChanged(() => this.MainHandVisibility);
            this.NotifyPropertyChanged(() => this.SeparatorVisibility);
            this.NotifyPropertyChanged(() => this.ClassSymbolUri);
            this.ignoreNextMainHandAddition = true;
        }

        private void OnEquippedOffHandIDChanged(object sender, ValueChangedEventArgs<int> e)
        {
            this.NotifyPropertyChanged(() => this.OverlayVisibility);
            this.NotifyPropertyChanged(() => this.OffHandVisibility);
            this.NotifyPropertyChanged(() => this.SeparatorVisibility);
            this.NotifyPropertyChanged(() => this.ClassSymbolUri);
            this.ignoreNextOffHandAddition = true;
        }

        public Uri ClassSymbolUri
        {

            get
            {

                if (this.glass != null)
                {
                    string className = null;

                    FFXIVItemSet itemSet = this.glass.ReadItemSet();

                    if (itemSet.Shield.ID == (int)FFXIVNovusWeaponID.HolyShieldNovus)
                    {
                        className = "paladin";
                    }
                    else
                    {
                        switch ((FFXIVNovusWeaponID)itemSet.Weapon.ID)
                        {
                            case FFXIVNovusWeaponID.CurtanaNovus:
                                className = "paladin";
                                break;
                            case FFXIVNovusWeaponID.SphairaiNovus:
                                className = "monk";
                                break;
                            case FFXIVNovusWeaponID.BravuraNovus:
                                className = "warrior";
                                break;
                            case FFXIVNovusWeaponID.GaeBolgNovus:
                                className = "dragoon";
                                break;
                            case FFXIVNovusWeaponID.ArtemisBowNovus:
                                className = "bard";
                                break;
                            case FFXIVNovusWeaponID.ThyrusNovus:
                                className = "whitemage";
                                break;
                            case FFXIVNovusWeaponID.StardustRodNovus:
                                className = "blackmage";
                                break;
                            case FFXIVNovusWeaponID.TheVeilofWiyuNovus:
                                className = "summoner";
                                break;
                            case FFXIVNovusWeaponID.OmnilexNovus:
                                className = "scholar";
                                break;
                            case FFXIVNovusWeaponID.YoshimitsuNovus:
                                className = "ninja";
                                break;
                        }
                    }

                    if (className != null)
                        return new Uri(string.Format("pack://application:,,,/Zodiac Glass;component/Resources/classimages/{0}.png", className));
                }

                return null;
            }

        }

        public string EquippedMainHandLightAmount
        {
            get
            {
                int val = 0;

                if (this.glass != null)
                {
                    FFXIVItemSet itemSet = this.glass.ReadItemSet();
                    val = itemSet.Weapon.LightAmount;
                }

                return this.Mode == OverlayDisplayMode.Normal ? val.ToString() : string.Format("{0} %", Math.Round(100 * (float)val / 2000, 2));
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

                return this.Mode == OverlayDisplayMode.Normal ? val.ToString() : string.Format("{0} %", Math.Round(100 * (float)val / 2000, 2));
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
                this.mainHandAddition = value;
                this.NotifyPropertyChanged(() => this.MainHandAddition);
                this.NotifyPropertyChanged(() => this.MainHandAdditionVisibility);
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
                this.offHandAddition = value;
                this.NotifyPropertyChanged(() => this.OffHandAddition);
                this.NotifyPropertyChanged(() => this.OffHandAdditionVisibility);
            }
        }
        public Visibility MainHandAdditionVisibility
        {
            get
            {
                return this.MainHandVisibility == Visibility.Visible && this.mainHandAddition > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility OffHandAdditionVisibility
        {
            get
            {
                return this.OffHandVisibility == Visibility.Visible && this.offHandAddition > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility SeparatorVisibility
        {
            get
            {
                return this.MainHandVisibility == Visibility.Visible && this.OffHandVisibility == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility MainHandVisibility
        {
            get
            {
                return this.glass != null && Enum.IsDefined(typeof(FFXIVNovusWeaponID), this.glass.ReadItemSet().Weapon.ID) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility OffHandVisibility
        {
            get
            {
                return this.glass != null && (FFXIVNovusWeaponID)this.glass.ReadItemSet().Shield.ID == FFXIVNovusWeaponID.HolyShieldNovus ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility OverlayVisibility
        {
            get
            {
                return this.ClassSymbolUri != null ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
