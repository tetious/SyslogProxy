namespace SyslogProxy
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using SyslogProxy.Messages;

    public class Proxy
    {
        private readonly Action<string> messageHandler;

        private readonly CancellationToken cancellationToken;

        private const int BufferSize = 2048;

        public Proxy(Action<string> messageHandler, CancellationToken cancellationToken)
        {
            this.messageHandler = messageHandler;
            this.cancellationToken = cancellationToken;
            var tcp = new TcpListener(IPAddress.Any, Configuration.ProxyPort);

            tcp.Start();
            this.AcceptConnection(tcp).ConfigureAwait(false);
        }

        private async Task AcceptConnection(TcpListener listener)
        {
            while (!this.cancellationToken.IsCancellationRequested)
            {
                var client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
                this.EchoAsync(client).ConfigureAwait(false);
            }
        }

        private async Task EchoAsync(TcpClient client)
        {
            var ipAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            Logger.Information("New client connected from IP: [{0}].", ipAddress);
            using (client)
            {
                var stream = client.GetStream();
                var buf = new byte[BufferSize];
                var accumulator = new StringBuilder();
                while (true)
                {
                    var timeoutTask = Task.Delay(TimeSpan.FromSeconds(Configuration.TcpConnectionTimeout));
                    Array.Clear(buf, 0, BufferSize);
                    var amountReadTask = stream.ReadAsync(buf, 0, buf.Length, this.cancellationToken);
                    var completedTask = await Task.WhenAny(timeoutTask, amountReadTask)
                                                  .ConfigureAwait(false);
                    if (completedTask == timeoutTask)
                    {
                        Logger.Information("Client with IP [{0}] timed out.", ipAddress);
                        break;
                    }

                    if (amountReadTask.Result == 0)
                    {
                        break;
                    }
                    accumulator.Append(Encoding.UTF8.GetString(buf).TrimEnd('\0'));
                    if (accumulator.ToString().Contains("\n"))
                    {
                        var splitMessage = accumulator.ToString().Split('\n').ToList();
                        accumulator = new StringBuilder(splitMessage.Last());
                        splitMessage.RemoveAt(splitMessage.Count - 1);
                        splitMessage.ForEach(this.messageHandler);
                    }
                }
            }
            Logger.Information("Client with IP [{0}] disconnected.", ipAddress);
        }
    }
}