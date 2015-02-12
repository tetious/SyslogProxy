namespace SyslogProxy
{
    using System.Collections;
    using System.Configuration.Install;
    using System.Diagnostics;

    public class EventSourceInstaller : Installer
    {
        public override void Install(IDictionary stateSaver)
        {
            if (!EventLog.SourceExists(Logger.EventSource))
            {
                EventLog.CreateEventSource(Logger.EventSource, "Application");
            }

            base.Install(stateSaver);
        }

        public override void Rollback(IDictionary savedState)
        {
            if (EventLog.SourceExists(Logger.EventSource))
            {
                
            }

            base.Rollback(savedState);
        }
    }
}