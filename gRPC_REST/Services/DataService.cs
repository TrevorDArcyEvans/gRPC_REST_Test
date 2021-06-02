using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace gRPC_REST.Services
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
      var response = new DataResponse
      {
        Message = "Requested:  " + request.Chunk
      };
      Enumerable.Range(0, request.Chunk).ToList().ForEach(i =>
      {
        var dp = new DataPacket
        {
          Name = "Name: " + Guid.NewGuid(),
          Id = Guid.NewGuid().ToString(),
          SourceId = Guid.NewGuid().ToString(),
          TargetId = Guid.NewGuid().ToString(),
          OrganisationId = Guid.NewGuid().ToString(),
          RepositoryId = Guid.NewGuid().ToString(),
          OwnerId = Guid.NewGuid().ToString(),
          NextId = Guid.NewGuid().ToString(),
        };
        response.Payload.Add(dp);
      });

      return Task.FromResult(response);
    }
  }
}
