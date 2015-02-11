namespace SyslogProxy.Messages
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Newtonsoft.Json;

    public class JsonSyslogMessage
    {
        private static readonly Regex DateStampRegex = new Regex(@"\w{3} \w{3}.{4}\d{2}:\d{2}:\d{2}.\d{3}");

        public JsonSyslogMessage(string rawMessage)
        {
            this.RawMessage = rawMessage;

            var splitLine = rawMessage.Split(' ');
            if (splitLine.Length < 4)
            {
                this.Invalid = true;
                return;
            }

            try
            {
                var priority = int.Parse(splitLine[0]);
                var facility = priority / 8;
                var severity = priority % 8;

                this.Facility = ((Facility)facility).ToString();
                this.Level = ((Severity)severity).ToString();
            }
            catch (Exception)
            {
                Logger.Warning("Could not parse priority. [{0}]", rawMessage);
                this.Invalid = true;
                return;
            }

            DateTime notUsed;
            this.Invalid = !DateTime.TryParse(splitLine[1], out notUsed);

            this.Timestamp = splitLine[1];
            this.Hostname = splitLine[2].Trim();
            this.ApplicationName = splitLine[3].Trim();
            this.Message = DateStampRegex.Replace(string.Join(" ", splitLine.Skip(4)).Trim(), string.Empty).Trim();
        }

        public bool Invalid { get; private set; }

        public string RawMessage { get; private set; }

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
                MessageTemplate = Configuration.MessageTemplate,
                Properties = new { this.Facility, this.Hostname, this.ApplicationName, this.Message }
            });
        }
    }
}
