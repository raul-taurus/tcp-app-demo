using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace tcp_server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = IPAddress.Parse("127.0.0.1");
            var port = 7000;
            var server = new TcpListener(host, port);
            server.Start(100);

            Console.WriteLine($"The server is listening on {host}:{port}");

            var sessionList = new List<Task>();

            var lifetime = TimeSpan.FromMinutes(1);
            var cts = new CancellationTokenSource(lifetime);
            var timeout = Task.Delay(lifetime);

            while (!cts.Token.IsCancellationRequested)
            {
                var listenerTask = server.AcceptTcpClientAsync();
                if (listenerTask == await Task.WhenAny(timeout, listenerTask))
                {
                    var client = await listenerTask;
                    Console.WriteLine($"Accept remote: {client.Client.RemoteEndPoint}");

                    var session = HandleConnection(client, cts.Token);
                    sessionList.Add(session);
                }
            }

            await Task.WhenAny(timeout, Task.WhenAll(sessionList));
        }

        static async Task HandleConnection(TcpClient client, CancellationToken cancellationToken)
        {
            using var stream = client.GetStream();
            using var reader = new StreamReader(stream);
            using var writer = new StreamWriter(stream);

            var msgs = new Queue<string>();
            const int msgLengthLimit = 5;

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var data = await reader.ReadLineAsync();
                    Console.WriteLine($"< {data}");

                    msgs.Enqueue(data);
                    if (msgs.Count > msgLengthLimit)
                    {
                        msgs.Dequeue();
                    }

                    await writer.WriteLineAsync("ok");
                    await writer.FlushAsync();

                    // Do what you want with the data queue.
                    // Console.WriteLine(string.Join(Environment.NewLine, msgs));
                }
            }
            finally
            {

            }
        }
    }
}
