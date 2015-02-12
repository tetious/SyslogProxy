namespace SyslogProxy
{
    using System.Diagnostics;

    public static class Logger
    {
        public const string EventSource = "SyslogProxy";

        public static void Information(string message, params object[] args)
        {
            Information(string.Format(message, args));
        }

        public static void Warning(string message, params object[] args)
        {
            Warning(string.Format(message, args));
        }

        public static void Error(string message, params object[] args)
        {
            Error(string.Format(message, args));
        }

        public static void Information(string message)
        {
            EventLog.WriteEntry(EventSource, message, EventLogEntryType.Information);
        }

        public static void Warning(string message)
        {
            EventLog.WriteEntry(EventSource, message, EventLogEntryType.Warning);
        }

        public static void Error(string message)
        {
            EventLog.WriteEntry(EventSource, message, EventLogEntryType.Error);            
        }
    }
}