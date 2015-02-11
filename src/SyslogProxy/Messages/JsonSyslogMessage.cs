namespace SyslogProxy.Messages
{
    using System.Linq;
    using System.Text.RegularExpressions;

    using Newtonsoft.Json;

    public class JsonSyslogMessage
    {
        private static readonly Regex DateStampRegex = new Regex(@"\w{3} \w{3}.{4}\d{2}:\d{2}:\d{2}.\d{3}");

        public JsonSyslogMessage(string syslogLine)
        {
            var splitLine = syslogLine.Split(' ');

            var priority = int.Parse(splitLine[0]);
            var facility = priority / 8;
            var severity = priority % 8;

            this.Facility = ((Facility) facility).ToString();
            this.Level = ((Severity) severity).ToString();

            this.Timestamp = splitLine[1];
            this.Hostname = splitLine[2].Trim();
            this.ApplicationName = splitLine[3].Trim();
            this.Message = DateStampRegex.Replace(string.Join(" ", splitLine.Skip(4)).Trim(), string.Empty).Trim();
        }

        public string Timestamp { get; set; }

        public string Level { get; set; }

        public string Facility { get; set; }

        public string Hostname { get; set; }
        
        public string ApplicationName { get; set; }
        
        public string Message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(new SeqEventMessage()
            {
                Level = this.Level,
                Timestamp = this.Timestamp,
                MessageTemplate = "{Hostname}:{ApplicationName} {Message}",
                Properties = new { this.Facility, this.Hostname, this.ApplicationName, this.Message }
            });
        }
    }
}
