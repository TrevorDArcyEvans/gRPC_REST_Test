using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Grpc.Net.Client;
using gRPC_REST;

namespace gRPC_REST_Client
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      // The port number(5001) must match the port of the gRPC server.
      using var channel = GrpcChannel.ForAddress("https://localhost:5001", new GrpcChannelOptions
      {
        MaxReceiveMessageSize = null, // null = remove limit!
        MaxSendMessageSize = 100 * 1024 * 1024 // 100 MB
      });
      var client = new DataService.DataServiceClient(channel);
      var warmupRequest = new DataRequest
      {
        Chunk = 2
      };
      _ = await client.GetDataAsync(warmupRequest);
      Console.WriteLine("Warmup call completed");

      var request = new DataRequest
      {
        Chunk = 1000000
      };
      var sw = Stopwatch.StartNew();
      var response = await client.GetDataAsync(request);
      var elapsedMs = sw.ElapsedMilliseconds;

      Console.WriteLine(response.Message);
      Console.WriteLine($"  {elapsedMs} ms");
      Console.WriteLine("Press any key to exit...");
      Console.ReadKey();
    }
  }
}
