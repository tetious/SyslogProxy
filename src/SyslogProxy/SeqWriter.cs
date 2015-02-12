namespace SyslogProxy
{
    using System;
    using System.Net.Http;
    using System.Runtime.ExceptionServices;
    using System.Text;
    using System.Threading.Tasks;

    using SyslogProxy.Messages;

    public class SeqWriter
    {
        private int retryCount;

        public async Task WriteToSeq(JsonSyslogMessage message, int delay = 0)
        {
            if (message.Invalid)
            {
                Logger.Warning("Skipping incomplete/invalid message. [{0}]", message.RawMessage);
                return;
            }

            await Task.Delay(Math.Min(delay, 60000));

            ExceptionDispatchInfo capturedException = null;
            try
            {
                await this.WriteMessage(message).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                capturedException = ExceptionDispatchInfo.Capture(ex);
            }

            if (capturedException != null)
            {
                this.retryCount++;
                Logger.Warning("Couldn't write to SEQ. Retry Count:[{0}] Exception: [{1}]", this.retryCount, capturedException.SourceException.Message);
                await this.WriteToSeq(message, (int)Math.Pow(100, this.retryCount));
            }
        }

        private async Task WriteMessage(JsonSyslogMessage message)
        {
            using (var http = new HttpClient())
            {
                using (var content = new StringContent("{\"events\":[" + message.ToString() + "]}", Encoding.UTF8, "application/json"))
                {
                    var response = await http.PostAsync(new Uri(Configuration.SeqServer, "api/events/raw"), content).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                }
            }
        }
    }
}