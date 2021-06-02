using System;
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
      using var channel = GrpcChannel.ForAddress("https://localhost:5001");
      var client = new Greeter.GreeterClient(channel);
      var request = new HelloRequest { Name = "GreeterClient" };
      var reply = await client.SayHelloAsync(request);

      Console.WriteLine("Greeting: " + reply.Message);
      Console.WriteLine("Press any key to exit...");
      Console.ReadKey();
    }
  }
}
