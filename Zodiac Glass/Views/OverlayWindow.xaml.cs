using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace ZodiacGlass
{
    /// <summary>
    /// Interaktionslogik für OverlayWindow.xaml
    /// </summary>
    public partial class OverlayWindow : Window
    {

        private readonly VirtualZodiacGlass glass;
        private readonly Process process;

        private Stopwatch mouseLeftButtonDownStopwatch;

        private IntPtr handle;

        public OverlayWindow(Process process = null)
        {
            this.process = process;

            if (this.process != null)
                this.glass = new VirtualZodiacGlass(process, ZodiacGlass.Properties.Settings.Default.MemoryMap ?? MemoryMap.Default);

            InitializeComponent();

            this.Left = Properties.Settings.Default.OverlayPosition.X;
            this.Top = Properties.Settings.Default.OverlayPosition.Y;

            (this.DataContext as OverlayViewModel).Glass = this.glass;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            if (this.process != null)
            {
                this.handle = new WindowInteropHelper(Application.Current.MainWindow).Handle;
                this.ReOwn();
            }
        }

        private void ReOwn()
        {
            NativeMethods.SetWindowLongPtr(this.handle, WindowLong.GWL_HWNDPARENT, this.process.MainWindowHandle);
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();

            Properties.Settings.Default.OverlayPosition = new System.Drawing.Point((int)this.Left, (int)this.Top);
            Properties.Settings.Default.Save();
        }

        private void OnImageMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.mouseLeftButtonDownStopwatch = Stopwatch.StartNew();
        }

        private void OnImageMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.mouseLeftButtonDownStopwatch.Stop();

            if (this.mouseLeftButtonDownStopwatch.ElapsedMilliseconds < 500)
            {
                OverlayViewModel vm = this.DataContext as OverlayViewModel;

                if (vm.Mode == OverlayViewModel.DisplayMode.Normal)
                    vm.Mode = OverlayViewModel.DisplayMode.Percentage;
                else
                    vm.Mode = OverlayViewModel.DisplayMode.Normal;
            }
        }
    }
}
