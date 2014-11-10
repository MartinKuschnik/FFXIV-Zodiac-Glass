namespace ZodiacGlass
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;
    using System.Xml.Serialization;
    using ZodiacGlass.FFXIV;
    using ContextMenu = System.Windows.Forms.ContextMenu;
    using MenuItem = System.Windows.Forms.MenuItem;
    using NotifyIcon = System.Windows.Forms.NotifyIcon;
    using ToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem;

    public partial class App : Application
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const string XIVProcessName = "ffxiv";

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Mutex mutex = new Mutex(true, "ZodiacGlass");

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool isMutexOwner;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Dictionary<Process, OverlayWindow> overlays = new Dictionary<Process, OverlayWindow>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ProcessObserver processObserver = new ProcessObserver();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly NotifyIcon notifyIcon = new NotifyIcon();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly FFXIVConfig xivConfig = new FFXIVConfig();

        protected override void OnStartup(StartupEventArgs e)
        {
            // ensure one one instance of app is open
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                this.isMutexOwner = true;

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
            if (this.isMutexOwner)
                mutex.ReleaseMutex();

            this.processObserver.Dispose();
            this.xivConfig.Dispose();

            foreach (Process process in this.overlays.Keys.ToArray())
                this.DestroyOverlay(process);

            base.OnExit(e);
        }

        #region NotifyIcon

        private void ConfigureNotifyIcon()
        {
            const string imageRuiFormat = "pack://application:,,,/Zodiac Glass;component/Resources/{0}";

            using (Stream iconStream = Application.GetResourceStream(new Uri(string.Format(imageRuiFormat, "ZodiacGlass.ico"))).Stream)
            {
                this.notifyIcon.Icon = new Icon(iconStream);
            }

            this.notifyIcon.Visible = true;

            this.notifyIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();

            if (!this.overlays.Any())
                this.notifyIcon.ShowBalloonTip(2500, "Zodiac Glass started.", "Please start FINAL FANTASY XIV: A Realm Reborn to see the overlay.", System.Windows.Forms.ToolTipIcon.Info);

            ToolStripMenuItem newItem;

            newItem = new ToolStripMenuItem("Bounus Light");
            newItem.Image = Image.FromStream(Application.GetResourceStream(new Uri(string.Format(imageRuiFormat, "reddit.png"))).Stream);
            newItem.Click += (s, e) => Process.Start("http://www.reddit.com/live/tlfmtjl4fteo");
            this.notifyIcon.ContextMenuStrip.Items.Add(newItem);

            newItem = new ToolStripMenuItem("Light Info");
            newItem.Image = Image.FromStream(Application.GetResourceStream(new Uri(string.Format(imageRuiFormat, "reddit.png"))).Stream);
            newItem.Click += (s, e) => Process.Start("http://www.reddit.com/r/ffxiv/comments/2gm1ru/nexus_light_information/");
            this.notifyIcon.ContextMenuStrip.Items.Add(newItem);

            newItem = new ToolStripMenuItem("About");
            newItem.Image = Image.FromStream(Application.GetResourceStream(new Uri(string.Format(imageRuiFormat, "about.ico"))).Stream);
            string msg;
            using (StreamReader sr = new StreamReader(Application.GetResourceStream(new Uri(string.Format(imageRuiFormat, "About.txt"))).Stream))
            {
                msg = sr.ReadToEnd();
            }
            newItem.Click += (s, e) => MessageBox.Show(msg, "About Zodiac Glass", MessageBoxButton.OK, MessageBoxImage.Information);
            this.notifyIcon.ContextMenuStrip.Items.Add(newItem);

            newItem = new ToolStripMenuItem("Donate");
            newItem.Image = Image.FromStream(Application.GetResourceStream(new Uri(string.Format(imageRuiFormat, "donate.ico"))).Stream);
            newItem.Click += (s, e) => Process.Start("https://www.paypal.com/cgi-bin/webscr?hosted_button_id=5MGV57U5FL728&cmd=_s-xclick");
            this.notifyIcon.ContextMenuStrip.Items.Add(newItem);

            newItem = new ToolStripMenuItem("Exit");
            newItem.Image = Image.FromStream(Application.GetResourceStream(new Uri(string.Format(imageRuiFormat, "exit.ico"))).Stream);
            newItem.Click += (s, e) => this.Shutdown();
            this.notifyIcon.ContextMenuStrip.Items.Add(newItem);

            XmlSerializer seri = new XmlSerializer(typeof(MemoryMap));

            using (FileStream fs = File.OpenWrite(@"C:\Users\Martin\Desktop\MemoryMap.xml"))
            {
                seri.Serialize(fs, MemoryMap.Default);
            }


        }

        #endregion

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
