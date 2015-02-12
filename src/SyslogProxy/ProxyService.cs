namespace SyslogProxy
{
    using System.Threading;

    using SimpleServices;

    using SyslogProxy.Messages;

    public class ProxyService : IWindowsService
    {
        public ApplicationContext AppContext { get; set; }

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public void Start(string[] args)
        {
            var writer = new SeqWriter();
            new Proxy(async message => await writer.WriteToSeq(new JsonSyslogMessage(message)).ConfigureAwait(false), 
                this.cancellationTokenSource.Token);
        }

        public void Stop()
        {
            this.cancellationTokenSource.Cancel();
        }
    }
}