namespace SyslogProxy
{
    using System.Linq;

    using Newtonsoft.Json;

    public class SyslogJson
    {
        public SyslogJson(string syslogLine)
        {
            var splitThing = syslogLine.Split(' ');

            var priority = int.Parse(splitThing[0]);
            var facility = priority / 8;
            var severity = priority % 8;

            this.Facility = ((Facility) facility).ToString();
            this.Level = ((Severity) severity).ToString();

            this.Timestamp = splitThing[1];
            this.Hostname = splitThing[2].Trim();
            this.ApplicationName = splitThing[3].Trim();
            this.Message = string.Join(" ", splitThing.Skip(4)).Trim();
        }

        public string Timestamp { get; set; }

        public string Level { get; set; }

        public string Facility { get; set; }

        public string Hostname { get; set; }
        
        public string ApplicationName { get; set; }
        
        public string Message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(new SeqEvent()
            {
                Level = this.Level,
                Timestamp = this.Timestamp,
                MessageTemplate = "{Hostname}.{Facility}:{ApplicationName} {Message}",
                Properties = new { this.Facility, this.Hostname, this.ApplicationName, this.Message }
            });
        }
    }
}
