namespace ZodiacGlass
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Threading;
    using ZodiacGlass.FFXIV;
    using ZodiacGlass.Native;
    using WindowStyle = ZodiacGlass.Native.WindowStyle;

    public partial class OverlayWindow : Window, IOverlay
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const int HWND_TOPMOST = -1;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Process process;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IntPtr handle;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mouseMovedAfterMouseLeftButtonDown;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Point initialPosition;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool pinned = true;

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

                    OverlayDisplayMode oldValue = this.ViewModel.Mode;

                    this.ViewModel.Mode = value;

                    if (this.DisplayModeChanged != null)
                        this.DisplayModeChanged(this, new ValueChangedEventArgs<OverlayDisplayMode>(oldValue, value));
                
            }
        }

        public bool Pinned
        {
            get
            {
                return this.pinned;
            }
            set
            {
                if (this.pinned != value)
                {
                    this.pinned = value;

                    this.UpdateWindow();
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

                    if (this.process != null && this.Pinned)
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
                    return this.initialPosition;
                }
            }

            set
            {
                Point oldPosition = this.Position;

                if (this.handle != IntPtr.Zero)
                {
                    if (this.pinned)
                    {
                        NativeMethods.SetWindowPos(this.handle, IntPtr.Zero, (int)value.X, (int)value.Y, (int)this.Width, (int)this.Height, SetWindowPosFlags.SWP_NONE);
                    }
                    else
                    {
                        NativeMethods.SetWindowPos(this.handle, (IntPtr)HWND_TOPMOST, (int)value.X, (int)value.Y, (int)this.Width, (int)this.Height, SetWindowPosFlags.SWP_NONE);
                    }
                }
                else
                {
                    this.initialPosition = value;
                }

                var positionChangedEvent = this.PositionChanged;

                if (positionChangedEvent != null)
                {
                    positionChangedEvent(this, new ValueChangedEventArgs<Point>(oldPosition, this.Position));
                }
            }
        }


        public Process Process
        {
            get { return this.process; }
        }

        public bool IsVisable
        {
            get
            {
                return (this.ViewModel as OverlayViewModel).IsOverlayVisible;
            }
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
            NativeMethods.SetWindowPos(this.handle, IntPtr.Zero, (int)initialPosition.X, (int)initialPosition.Y, (int)this.Width, (int)this.Height, SetWindowPosFlags.SWP_NONE);

            this.UpdateWindow();
        }
        
        private void UpdateWindow()
        {
            if (this.handle != IntPtr.Zero)
            {
                WindowStyle style;

                if (this.pinned)                
                {
                    this.Topmost = false;
                    style = Native.WindowStyle.WS_CLIPSIBLINGS | Native.WindowStyle.WS_CLIPCHILDREN | Native.WindowStyle.WS_SYSMENU | Native.WindowStyle.WS_CHILD | Native.WindowStyle.WS_VISIBLE;
                    NativeMethods.SetParent(this.handle, this.process.MainWindowHandle);

                    // pin the overlay window over the game window
                    NativeMethods.SetWindowLong(handle, WindowLong.GWL_STYLE, (IntPtr)style);
                    NativeMethods.SetWindowLong(handle, WindowLong.GWL_EXSTYLE, (IntPtr)WindowStyleEx.WS_EX_NOACTIVATE);
                }
                else
                {
                    this.Topmost = true;
                    style = Native.WindowStyle.WS_POPUP | Native.WindowStyle.WS_VISIBLE;
                    NativeMethods.SetParent(this.handle, IntPtr.Zero);

                    NativeMethods.SetWindowLong(handle, WindowLong.GWL_STYLE, (IntPtr)style);
                    NativeMethods.SetWindowLong(handle, WindowLong.GWL_EXSTYLE, (IntPtr)(WindowStyleEx.WS_EX_LAYERED));

                    NativeMethods.SetWindowPos(this.handle, (IntPtr)HWND_TOPMOST, (int)this.Left, (int)this.Top, (int)this.Width, (int)this.Height, SetWindowPosFlags.SWP_NONE);
                }

                this.Invalidate();
            }
        }

        private void Invalidate()
        {
            // no idea why but this works (ToDo: find a better way)

            OverlayDisplayMode resetValue = this.DisplayMode;

            if (this.DisplayMode == OverlayDisplayMode.Normal)
            {
                this.DisplayMode = OverlayDisplayMode.Percentage;
            }
            else
            {
                this.DisplayMode = OverlayDisplayMode.Normal;
            }

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(10);
                this.dispatcher.Invoke((ThreadStart)(() => this.DisplayMode = resetValue));
            });
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


        public void Highlight(int sec)
        {
            Brush startBackground = this.Background;
            Brush highlightColor = new SolidColorBrush(Color.FromArgb(150, 255, 0, 0));

            Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < sec * 2; i++)
                {
                    Thread.Sleep(500);

                    if (i % 2 == 0)
                    {
                        this.dispatcher.Invoke((ThreadStart)(() => this.Background = highlightColor));
                    }
                    else
                    {
                        this.dispatcher.Invoke((ThreadStart)(() => this.Background = startBackground));
                    }

                }
            });
        }
    }
}
