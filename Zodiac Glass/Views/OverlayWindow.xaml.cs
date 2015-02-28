namespace ZodiacGlass
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interop;
    using ZodiacGlass.FFXIV;
    using ZodiacGlass.Native;
    using WindowStyle = ZodiacGlass.Native.WindowStyle;

    public partial class OverlayWindow : Window, IOverlay
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Process process;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IntPtr handle;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mouseMovedAfterMouseLeftButtonDown;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Point initialRelativePosition;

        public OverlayWindow(Process process)
        {
            if (process == null)
                throw new ArgumentNullException(MethodBase.GetCurrentMethod().GetParameters()[0].Name);

            this.process = process;

            InitializeComponent();
        }

        public event EventHandler<ValueChangedEventArgs<OverlayDisplayMode>> DisplayModeChanged;

        public event EventHandler<ValueChangedEventArgs<Point>> PositionChanged;

        public OverlayDisplayMode DisplayMode
        {
            get
            {
                return this.ViewModel.Mode;
            }
            set
            {
                if (this.ViewModel.Mode != value)
                {
                    OverlayDisplayMode oldValue = this.ViewModel.Mode;

                    this.ViewModel.Mode = value;

                    if (this.DisplayModeChanged != null)
                        this.DisplayModeChanged(this, new ValueChangedEventArgs<OverlayDisplayMode>(oldValue, value));
                }
            }
        }

        public Point Position
        {
            get
            {
                if (this.handle != IntPtr.Zero)
                {
                    RECT oberlayWindowRect;
                    RECT gameWindowRect = default(RECT);

                    WindowStyle gameWindowStye = Native.WindowStyle.WS_OVERLAPPED;

                    oberlayWindowRect = NativeMethods.GetWindowRect(this.handle);

                    if (this.process != null)
                    {
                        gameWindowRect = NativeMethods.GetWindowRect(this.process.MainWindowHandle);

                        gameWindowStye = (Native.WindowStyle)NativeMethods.GetWindowLong(this.process.MainWindowHandle, WindowLong.GWL_STYLE);
                    }

                    if (gameWindowStye.HasFlag(Native.WindowStyle.WS_THICKFRAME))
                    {
                        return new Point(oberlayWindowRect.Left - gameWindowRect.Left - SystemParameters.BorderWidth, oberlayWindowRect.Top - gameWindowRect.Top - SystemParameters.CaptionHeight);
                    }
                    else
                    {
                        return new Point(oberlayWindowRect.Left - gameWindowRect.Left, oberlayWindowRect.Top - gameWindowRect.Top);
                    }
                }
                else
                {
                    return this.initialRelativePosition;
                }
            }

            set
            {
                Point oldPosition = this.Position;

                if (oldPosition != value)
                {
                    if (this.handle != IntPtr.Zero)
                    {
                        NativeMethods.SetWindowPos(this.handle, IntPtr.Zero, (int)value.X, (int)value.Y, (int)this.Width, (int)this.Height, SetWindowPosFlags.SWP_NONE);
                    }
                    else
                    {
                        this.initialRelativePosition = value;
                    }

                    var positionChangedEvent = this.PositionChanged;

                    if (positionChangedEvent != null)
                    {
                        positionChangedEvent(this, new ValueChangedEventArgs<Point>(oldPosition, this.Position));
                    }
                }
            }
        }
        
        public Process Process
        {
            get { return this.process; }
        }        

        FFXIVMemoryReader IOverlay.MemoryReader
        {
            get
            {
                return this.ViewModel.MemoryReader;
            }
            set
            {
                this.ViewModel.MemoryReader = value;
            }
        }

        private OverlayViewModel ViewModel
        {
            get
            {
                return (this.DataContext as OverlayViewModel);
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            this.handle = new WindowInteropHelper(this).Handle;

            // set initial pos
            NativeMethods.SetWindowPos(this.handle, IntPtr.Zero, (int)initialRelativePosition.X, (int)initialRelativePosition.Y, (int)this.Width, (int)this.Height, SetWindowPosFlags.SWP_NONE);

            // pin the overlay window over the game window
            WindowStyle style = (WindowStyle)NativeMethods.GetWindowLong(handle, WindowLong.GWL_STYLE);
            NativeMethods.SetWindowLong(handle, WindowLong.GWL_STYLE, (IntPtr)(style |= ZodiacGlass.Native.WindowStyle.WS_CHILD));
            NativeMethods.SetWindowLong(handle, WindowLong.GWL_EXSTYLE, (IntPtr)WindowStyleEx.WS_EX_NOACTIVATE);

            NativeMethods.SetParent(this.handle, this.process.MainWindowHandle);
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.mouseMovedAfterMouseLeftButtonDown = false;
            this.MouseMove += this.OnMouseMoveAfterMouseLeftButtonDown;
        }

        private void OnMouseMoveAfterMouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            this.mouseMovedAfterMouseLeftButtonDown = true;
            this.MouseMove -= this.OnMouseMoveAfterMouseLeftButtonDown;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point oldPosition = this.Position;

                this.DragMove();

                var positionChangedEvent = this.PositionChanged;

                if (positionChangedEvent != null)
                {
                    positionChangedEvent(this, new ValueChangedEventArgs<Point>(oldPosition, this.Position));
                }
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.MouseMove -= this.OnMouseMoveAfterMouseLeftButtonDown;

            if (!this.mouseMovedAfterMouseLeftButtonDown)
            {
                if (this.DisplayMode == OverlayDisplayMode.Normal)
                    this.DisplayMode = OverlayDisplayMode.Percentage;
                else
                    this.DisplayMode = OverlayDisplayMode.Normal;
            }
        }
    }
}
