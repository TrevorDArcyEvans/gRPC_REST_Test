using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Grpc.Net.Client;
using gRPC_REST;
using RestSharp;

namespace gRPC_REST_Client
{
  public class Program
  {
    private const int ChunkSize = 1000000;

    public static async Task Main(string[] args)
    {
      Console.WriteLine("Testing gRPC...");
      await Test_gRPC();

      Console.WriteLine("Testing REST...");
      await Test_REST();

      Console.WriteLine("Press any key to exit...");
      Console.ReadKey();
    }

    private static async Task Test_gRPC()
    {
      // The port number(5001) must match the port of the gRPC server.
      using var channel = GrpcChannel.ForAddress("http://localhost:5001", new GrpcChannelOptions
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
      Console.WriteLine("  Warmup call completed");

      var request = new DataRequest
      {
        Chunk = ChunkSize
      };
      var sw = Stopwatch.StartNew();
      var response = await client.GetDataAsync(request);
      var elapsedMs = sw.ElapsedMilliseconds;

      Console.WriteLine(response.Message);
      Console.WriteLine($"  {elapsedMs} ms");
      Console.WriteLine();
    }

    private static async Task Test_REST()
    {
      var client = new RestClient("http://localhost:4999");
      var warmupRequest = new RestRequest("/api/DataService/GetData/2", DataFormat.Json);
      var warmupResponse = await client.GetAsync<DataResponseDTO>(warmupRequest);
      Console.WriteLine("  Warmup call completed");
      
      var request = new RestRequest($"/api/DataService/GetData/{ChunkSize}", DataFormat.Json);
      var sw = Stopwatch.StartNew();
      var response = await client.GetAsync<DataResponseDTO>(request);
      var elapsedMs = sw.ElapsedMilliseconds;

      Console.WriteLine(response.Message);
      Console.WriteLine($"  {elapsedMs} ms");
      Console.WriteLine();
    }
  }
}
