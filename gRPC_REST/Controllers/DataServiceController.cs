using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace gRPC_REST.Controllers
{
  [Route("api/[controller]")]
  [Produces("application/json")]
  public class DataServiceController : ControllerBase
  {
    [HttpGet]
    [Route(nameof(GetData) + "/{chunk:int}")]
    public ActionResult<DataResponse> GetData(
      [FromRoute] [Required] int chunk)
    {
      var response = new DataResponse
      {
        Message = "Requested:  " + chunk
      };
      Enumerable.Range(0, chunk).ToList().ForEach(i =>
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

      return Ok(response);
    }
  }
}
