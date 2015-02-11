namespace SyslogProxy.Messages
{
    using System;
    using System.Configuration;

    public static class Configuration
    {
        public static Uri SeqServer
        {
            get
            {
                var seqServer = ConfigurationManager.AppSettings["SeqServer"];
                return string.IsNullOrWhiteSpace(seqServer) ? null : new Uri(seqServer);
            }
        }

        public static string MessageTemplate
        {
            get
            {
                return ConfigurationManager.AppSettings["MessageTemplate"];
            }
        }

        public static int ProxyPort
        {
            get
            {
                return IntOrDefault(ConfigurationManager.AppSettings["ProxyPort"], 6514);
            }
        }

        public static int TcpConnectionTimeout
        {
            get
            {
                return IntOrDefault(ConfigurationManager.AppSettings["TcpConnectionTimeout"], 60 * 10);
            }
        }

        public static void Validate()
        {
            if (SeqServer == null)
            {
                throw new InvalidOperationException("SeqServer is null or empty.");
            }

            if (string.IsNullOrWhiteSpace(MessageTemplate))
            {
                throw new InvalidOperationException("MessageTemplate is null or empty.");
            }
        }

        private static int IntOrDefault(string candidate, int orDefault)
        {
            int parsedInt;
            return int.TryParse(candidate, out parsedInt) ? parsedInt : orDefault;
        }
    }
}