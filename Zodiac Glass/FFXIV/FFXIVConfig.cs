namespace ZodiacGlass.FFXIV
{
    using System.Diagnostics;

    internal struct FFXIVConfig
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private FFXIVScreenMode? screenMode;

        public FFXIVScreenMode ScreenMode
        {
            get { return this.screenMode ?? FFXIVScreenMode.Unknwon; }
            set { this.screenMode = value; }
        }        

    }
}
