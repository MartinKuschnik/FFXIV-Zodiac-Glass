namespace ZodiacGlass
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;
    using System.Xml.Serialization;
    using ZodiacGlass.FFXIV;
    using ContextMenu = System.Windows.Forms.ContextMenu;
    using MenuItem = System.Windows.Forms.MenuItem;
    using NotifyIcon = System.Windows.Forms.NotifyIcon;
    using ToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem;
    using Settings = ZodiacGlass.Properties.Settings;

    public partial class App : Application
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const string XIVProcessName = "ffxiv";

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Mutex mutex = new Mutex(true, "ZodiacGlass");

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Log log = new Log();

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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool isMutexOwner;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MemoryMap currentMemoryMap;

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                // ensure one one instance of app is open
                if (mutex.WaitOne(TimeSpan.Zero, true))
                {
                    this.isMutexOwner = true;

                    base.OnStartup(e);

                    try
                    {
                        this.currentMemoryMap = Settings.Default.MemoryMap ?? MemoryMap.Default;
                    }
                    catch (Exception ex)
                    {
                        this.currentMemoryMap = MemoryMap.Default;
                        this.log.WriteException("Reading MemoryMap failed.", ex);
                    }

                    this.UpdateMemoryMap();
                    
                    this.processObserver.ProcessStarted += this.OnProcessStarted;

                    this.AttachToExistingProcesses();

                    this.ConfigureNotifyIcon();
                }
                else
                {
                    MessageBox.Show("Only one instance of Zodiac Glass be run!", "Zodiac Glass", MessageBoxButton.OK, MessageBoxImage.Warning);

                    this.log.Write(LogLevel.Info, "Zodiac Glass already running. Application will be closed.");

                    this.Shutdown();
                }
            }
            catch (Exception ex)
            {
                this.log.WriteException("Startup failed.", ex);
                throw;
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            foreach (Process process in this.overlays.Keys.ToArray())
                this.DestroyOverlay(process);

            this.processObserver.Dispose();
            this.xivConfig.Dispose();
            this.log.Dispose();

            if (this.isMutexOwner)
                mutex.ReleaseMutex();

            base.OnExit(e);
        }
        
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
        }
        
        private bool UpdateMemoryMap()
        {
            try
            {
                WebRequest request = HttpWebRequest.Create("https://raw.githubusercontent.com/InvisibleShield/FFXIV-Zodiac-Glass/master/CurrentMemoryMap.xml");

                using (Stream responseStream = request.GetResponse().GetResponseStream())
                {
                    XmlSerializer seri = new XmlSerializer(typeof(MemoryMap));

                    MemoryMap newMemMap = seri.Deserialize(responseStream) as MemoryMap;

                    if (newMemMap != null && !newMemMap.Equals(this.currentMemoryMap))
                    {
                        this.currentMemoryMap = newMemMap;

                        try
                        {
                            Settings.Default.MemoryMap = newMemMap;
                            Settings.Default.Save();

                            this.log.Write(LogLevel.Info, "MemoryMap updated.");
                        }
                        catch (Exception ex)
                        {
                            this.log.WriteException("Saving new MemoryMap failed.", ex);
                        }

                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                this.log.WriteException("Updating MemoryMap failed.", ex);
                return false;
            }
        }

        #region Overlay

        private void AttachToExistingProcesses()
        {
            foreach (Process process in Process.GetProcessesByName(App.XIVProcessName))
                this.CreateOverlay(process);
        }

        private void CreateOverlay(Process process)
        {
            this.log.Write(LogLevel.Trace, string.Format("Creating overlay for process {0}.", process.Id));

            try
            {
                OverlayWindow overlay = new OverlayWindow(process);

                process.EnableRaisingEvents = true;
                process.Exited += this.OnProcessExited;

                overlay.Show();
                this.overlays.Add(process, overlay);

                this.CheckGameIsInWindowMode(process);
            }
            catch (Exception ex)
            {
                this.log.WriteException("Creating overlay failed.", ex);
            }
        }

        private bool DestroyOverlay(Process process)
        {
            OverlayWindow overlay;

            if (this.overlays.TryGetValue(process, out overlay))
            {
                process.Exited -= this.OnProcessExited;

                this.log.Write(LogLevel.Trace, string.Format("Destroying overlay for process {0}.", process.Id));

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

        private void CheckGameIsInWindowMode(Process process)
        {
            ScreenMode curScreanMode = this.xivConfig.ScreenMode;

            this.log.Write(LogLevel.Info, string.Format("CurrentScreenMode: {0}", curScreanMode));

            if (curScreanMode != ScreenMode.FramelessWindow)
                MessageBox.Show("FINAL FANTASY XIV have to run into frameless window mode!", "Frameless window mode required!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        #endregion
    }
}
