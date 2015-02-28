namespace ZodiacGlass
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using ZodiacGlass.FFXIV;

    interface IOverlay
    {
        event EventHandler<ValueChangedEventArgs<OverlayDisplayMode>> DisplayModeChanged;

        event EventHandler<ValueChangedEventArgs<Point>> PositionChanged;

        OverlayDisplayMode DisplayMode { get; set; }

        Point Position { get; set; }

        Process Process { get; }

        FFXIVMemoryReader MemoryReader { get; set; }

        void Show();

        void Close();
    }
}
