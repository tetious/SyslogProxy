namespace SyslogProxy
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    using SyslogProxy.Messages;

    public class Proxy
    {
        private readonly Action<string> messageHandler;

        public Proxy(Action<string> messageHandler)
        {
            this.messageHandler = messageHandler;
            var tcp = new TcpListener(IPAddress.Any, Configuration.ProxyPort);

            tcp.Start();
            this.AcceptConnection(tcp).ConfigureAwait(false);
        }

        private async Task AcceptConnection(TcpListener listener)
        {
            while (true)
            {
                var client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
                this.EchoAsync(client).ConfigureAwait(false);
            }
        }

        private async Task EchoAsync(TcpClient client)
        {
            Console.WriteLine("New client connected.");
            using (client)
            {
                var buf = new byte[4096];
                var stream = client.GetStream();
                while (true)
                {
                    var timeoutTask = Task.Delay(TimeSpan.FromSeconds(Configuration.TcpConnectionTimeout));
                    var amountReadTask = stream.ReadAsync(buf, 0, buf.Length);
                    var completedTask = await Task.WhenAny(timeoutTask, amountReadTask)
                                                  .ConfigureAwait(false);
                    if (completedTask == timeoutTask)
                    {
                        Console.WriteLine("Client timed out");
                        break;
                    }

                    var amountRead = amountReadTask.Result;
                    if (amountRead == 0)
                    {
                        break;
                    }

                    this.messageHandler(Encoding.UTF8.GetString(buf).TrimEnd('\0'));
                }
            }
            Console.WriteLine("Client disconnected");
        }
    }
}