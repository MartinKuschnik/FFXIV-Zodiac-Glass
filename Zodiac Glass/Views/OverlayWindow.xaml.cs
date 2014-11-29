namespace ZodiacGlass
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interop;
    using ZodiacGlass.FFXIV;
    using ZodiacGlass.Native;

    public partial class OverlayWindow : Window
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Process process;
        
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IntPtr handle;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool mouseMovedAfterMouseLeftButtonDown;
        
        public OverlayWindow(Process process)
        {
            this.process = process;

            InitializeComponent();
        }

        internal event EventHandler<ValueChangedEventArgs<OverlayDisplayMode>> DisplayModeChanged;

        internal OverlayDisplayMode DisplayMode
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

        private OverlayViewModel ViewModel
        {
            get
            {
                return (this.DataContext as OverlayViewModel);
            }
        }

        internal FFXIVMemoryReader MemoryReader
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

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            if (this.process != null)
            {
                this.handle = new WindowInteropHelper(Application.Current.MainWindow).Handle;
                this.SetProcessWindowAsOwner();
            }
        }

        private void SetProcessWindowAsOwner()
        {
            NativeMethods.SetWindowLongPtr(this.handle, WindowLong.GWL_HWNDPARENT, this.process.MainWindowHandle);
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
                this.DragMove();
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
