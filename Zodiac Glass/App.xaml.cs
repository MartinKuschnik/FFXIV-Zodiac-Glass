namespace ZodiacGlass
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;
    using NotifyIcon = System.Windows.Forms.NotifyIcon;
    using ContextMenu = System.Windows.Forms.ContextMenu;
    using MenuItem = System.Windows.Forms.MenuItem;
    using System.Threading;
    using ZodiacGlass.FFXIV;

    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private const string XIVProcessName = "ffxiv";

        private readonly Mutex mutex = new Mutex(true, "ZodiacGlass");

        private bool isMutexOwner;

        private readonly Dictionary<Process, OverlayWindow> overlays = new Dictionary<Process, OverlayWindow>();

        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        private readonly ProcessObserver processObserver = new ProcessObserver();

        private readonly NotifyIcon notifyIcon = new NotifyIcon();

        private readonly FFXIVConfig xivConfig = new FFXIVConfig();

        protected override void OnStartup(StartupEventArgs e)
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                isMutexOwner = true;

                base.OnStartup(e);

                this.processObserver.ProcessStarted += this.OnProcessStarted;

                this.AttachToExistingProcesses();

                this.ConfigureNotifyIcon();
            }
            else
            {
                MessageBox.Show("Only one instance of Zodiac Glass be run!", "Zodiac Glass", MessageBoxButton.OK, MessageBoxImage.Warning);

                this.Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (isMutexOwner)
                mutex.ReleaseMutex();

            this.processObserver.Dispose();
            this.xivConfig.Dispose();

            foreach (Process process in this.overlays.Keys.ToArray())
                this.DestroyOverlay(process);

            base.OnExit(e);
        }

        private void ConfigureNotifyIcon()
        {
            Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/Zodiac Glass;component/Resources/ZodiacGlass.ico")).Stream;

            if (iconStream != null)
                this.notifyIcon.Icon = new Icon(iconStream);

            this.notifyIcon.Visible = true;

            this.notifyIcon.ContextMenu = new ContextMenu();

            MenuItem exitMenuItem = this.notifyIcon.ContextMenu.MenuItems.Add("Exit", this.OnExitContextMenuItemClicked);

            
        }

        private void OnExitContextMenuItemClicked(object sender, EventArgs e)
        {
            this.Shutdown();
        }

        private void AttachToExistingProcesses()
        {
            foreach (Process process in Process.GetProcessesByName(App.XIVProcessName))
                this.CreateOverlay(process);
        }

        private void CreateOverlay(Process process)
        {
            OverlayWindow overlay = new OverlayWindow(process);

            process.EnableRaisingEvents = true;
            process.Exited += this.OnProcessExited;

            overlay.Show();

            this.overlays.Add(process, overlay);

            this.CheckGameIsInWindowMode(process);
        }

        private void CheckGameIsInWindowMode(Process process)
        {
            if (this.xivConfig.ScreenMode != ScreenMode.FramelessWindow)
                MessageBox.Show("FINAL FANTASY XIV have to run into frameless window mode!", "Frameless window mode required!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private bool DestroyOverlay(Process process)
        {
            OverlayWindow overlay;

            if (this.overlays.TryGetValue(process, out overlay))
            {
                process.Exited -= this.OnProcessExited;

                overlay.Close();

                return this.overlays.Remove(process);
            }

            return false;
        }

        private void OnProcessStarted(object sender, ProcessEventArgs e)
        {
            if (e.Process.ProcessName == App.XIVProcessName)
                dispatcher.Invoke((ThreadStart)(() => this.CreateOverlay(e.Process)));
        }

        private void OnProcessExited(object sender, EventArgs e)
        {
            Process process = sender as Process;

            if (process != null)
                dispatcher.Invoke((ThreadStart)(() => this.DestroyOverlay(process)));   
        }
    }
}
