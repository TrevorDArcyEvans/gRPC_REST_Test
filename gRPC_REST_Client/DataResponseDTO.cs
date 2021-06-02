using System.Collections.Generic;
using gRPC_REST;

namespace gRPC_REST_Client
{
  public class DataResponseDTO
  {
    public string Message { get; set; }
    
    public List<DataPacket> Payload { get; set; }
  }
}
