using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace gRPC_REST
{
  public class DataServiceImpl : DataService.DataServiceBase
  {
    private readonly ILogger<DataServiceImpl> _logger;

    public DataServiceImpl(ILogger<DataServiceImpl> logger)
    {
      _logger = logger;
    }

    public override Task<DataResponse> GetData(DataRequest request, ServerCallContext context)
    {
      return Task.FromResult(new DataResponse
      {
        Message = "Hello " + request.Chunk.ToString()
      });
    }
  }
}
