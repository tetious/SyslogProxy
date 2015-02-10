namespace SyslogProxy
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class Program
    {
        static void Main(string[] args)
        {
            var tcp = new TcpListener(IPAddress.Any, 6514);

            tcp.Start();
            AcceptConnection(tcp);
            while (true)
            {
                Thread.Sleep(10000);
            }
        }

        static async Task AcceptConnection(TcpListener listener)
        {
            while (true)
            {
                var client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
                EchoAsync(client);
            }
        }

        static async Task EchoAsync(TcpClient client)
        {
            Console.WriteLine("New client connected.");
            using (client)
            {
                var buf = new byte[4096];
                var stream = client.GetStream();
                while (true)
                {
                    var timeoutTask = Task.Delay(TimeSpan.FromSeconds(15));
                    var amountReadTask = stream.ReadAsync(buf, 0, buf.Length);
                    var completedTask = await Task.WhenAny(timeoutTask, amountReadTask)
                                                  .ConfigureAwait(false);
                    if (completedTask == timeoutTask)
                    {
                        Console.WriteLine("Client timed out");
                        break;
                    }

                    var amountRead = amountReadTask.Result;
                    if (amountRead == 0) break; //end of stream.
                    // stuff buff into a json thing
                    await WriteToSeq(new SyslogJson(Encoding.UTF8.GetString(buf).TrimEnd('\0')));
                }
            }
            Console.WriteLine("Client disconnected");
        }

        public static async Task WriteToSeq(SyslogJson syslog)
        {
            using (var http = new HttpClient())
            {
                using (var content = new StringContent("{\"events\":[" +syslog.ToString() + "]}", Encoding.UTF8, "application/json"))
                {
                    var response = await http.PostAsync("http://10.2.10.156:5341/api/events/raw", content);
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("ERROR: Could not send to SEQ.");
                    }
                }
            }
        }
    }
}
