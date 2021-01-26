using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace tcp_client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = "127.0.0.1";
            var port = 7000;

            var lifetime = TimeSpan.FromMinutes(1);
            var cts = new CancellationTokenSource(lifetime);
            var timeout = Task.Delay(lifetime);

            var clients = Enumerable.Range(1, 10).Select(i => ConnectServer(host, port, i, cts.Token)).ToList();

            await Task.WhenAny(timeout, Task.WhenAll(clients));
        }

        static async Task ConnectServer(string host, int port, int userId, CancellationToken cancellationToken)
        {
            var client = new TcpClient();
            await client.ConnectAsync(host, port);
            Console.WriteLine($"Local: {client.Client.LocalEndPoint}");

            using var stream = client.GetStream();
            using var reader = new StreamReader(stream);
            using var writer = new StreamWriter(stream);

            int sequence = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                var data = $"#station-{userId:D2}/data-{++sequence:D2}";

                await writer.WriteLineAsync(data);
                await writer.FlushAsync();

                var resp = await reader.ReadLineAsync();
                Console.WriteLine($"> {data} -> {resp}");

                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }
    }
}
