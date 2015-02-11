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
                Console.Write("Couldn't write to SEQ. Exception: [{0}]", capturedException.SourceException.Message);
                this.retryCount++;
                await this.WriteToSeq(message, 100 ^ this.retryCount);
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