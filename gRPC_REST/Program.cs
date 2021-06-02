using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;

namespace gRPC_REST
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      await CreateHostBuilder(args).Build().RunAsync();
    }

    // Additional configuration is required to successfully run gRPC on macOS.
    // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder => webBuilder.ConfigureKestrel(options =>
          {
            options.ListenAnyIP(4999, listenOptions => listenOptions.Protocols = HttpProtocols.Http1);

            options.ListenAnyIP(5001, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
          })
          .UseStartup<Startup>());
  }
}
