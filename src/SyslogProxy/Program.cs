namespace SyslogProxy
{
    using System.Threading;

    using SyslogProxy.Messages;

    public class Program
    {
        static void Main(string[] args)
        {
            var writer = new SeqWriter();
            
            new Proxy(async message => await writer.WriteToSeq(new JsonSyslogMessage(message)).ConfigureAwait(false));

            // TODO: do something better than loop endlessly
            while (true)
            {
                Thread.Sleep(100000);
            }
        }
    }
}
