namespace ZodiacGlass
{
    using System;
    using System.Diagnostics;
    using System.Management;

    internal class ProcessObserver : IDisposable
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private readonly ManagementEventWatcher watcher;

            public event EventHandler<ProcessEventArgs> ProcessStarted;

            public ProcessObserver()
            {
                watcher = new ManagementEventWatcher();
                watcher.Query = new WqlEventQuery("__InstanceCreationEvent", new TimeSpan(0, 0, 1), "TargetInstance isa \"Win32_Process\""); ;
                watcher.EventArrived += new EventArrivedEventHandler(OnProcessStarted);
                watcher.Start();
            }

            private void OnProcessStarted(object sender, EventArrivedEventArgs e)
            {
                try
                {
                    uint processId = (uint)(e.NewEvent["TargetInstance"] as ManagementBaseObject).GetPropertyValue("ProcessId");
                    string eventType = e.NewEvent.ClassPath.ClassName;

                    ProcessEventArgs args = new ProcessEventArgs(Process.GetProcessById((int)processId));
                    EventHandler<ProcessEventArgs> handler;

                    switch (eventType)
                    {
                        case "__InstanceCreationEvent":
                            handler = this.ProcessStarted;
                            if (handler != null)
                                this.ProcessStarted(this, args);
                            break;

                        case "__InstanceDeletionEvent":
                            break;

                        case "__InstanceModificationEvent":
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }

            }

            public void Dispose()
            {
                watcher.Stop();
            }
        }

}
