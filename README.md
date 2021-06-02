# gRPC vs REST

Based on code from:

[Tutorial: Create a gRPC client and server in ASP.NET Core](https://docs.microsoft.com/en-gb/aspnet/core/tutorials/grpc/grpc-start?view=aspnetcore-5.0&tabs=visual-studio)

[Troubleshoot gRPC on .NET Core](https://docs.microsoft.com/en-gb/aspnet/core/grpc/troubleshoot?view=aspnetcore-5.0)

[Create Protobuf messages for .NET apps](https://docs.microsoft.com/en-us/aspnet/core/grpc/protobuf?view=aspnetcore-5.0)

## Results

### Client

<details>

```bash
Testing gRPC...
  Warmup call completed
Requested:  1000000
  14716 ms

Testing REST...
  Warmup call completed
Requested:  1000000
  120629 ms

Press any key to exit...
```

</details>

### Server

<details>

```bash
warn: Microsoft.AspNetCore.Server.Kestrel[0]
      Overriding address(es) 'http://localhost:5000, https://localhost:5001'. Binding to endpoints defined in UseKestrel() instead.
info: Microsoft.Hosting.Lifetime[0]
      Now listening on: http://[::]:4999
info: Microsoft.Hosting.Lifetime[0]
      Now listening on: http://[::]:5001
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: C:\dev\trevorde\gRPC_REST_Test\gRPC_REST
info: Microsoft.AspNetCore.Hosting.Diagnostics[1]
      Request starting HTTP/2 POST http://localhost:5001/Data.DataService/GetData application/grpc -
info: Microsoft.AspNetCore.Routing.EndpointMiddleware[0]
      Executing endpoint 'gRPC - /Data.DataService/GetData'
info: Microsoft.AspNetCore.Routing.EndpointMiddleware[1]
      Executed endpoint 'gRPC - /Data.DataService/GetData'
info: Microsoft.AspNetCore.Hosting.Diagnostics[2]
      Request finished HTTP/2 POST http://localhost:5001/Data.DataService/GetData application/grpc - - 200 - application/grpc 245.0840ms
info: Microsoft.AspNetCore.Hosting.Diagnostics[1]
      Request starting HTTP/2 POST http://localhost:5001/Data.DataService/GetData application/grpc -
info: Microsoft.AspNetCore.Routing.EndpointMiddleware[0]
      Executing endpoint 'gRPC - /Data.DataService/GetData'
info: Microsoft.AspNetCore.Routing.EndpointMiddleware[1]
      Executed endpoint 'gRPC - /Data.DataService/GetData'
info: Microsoft.AspNetCore.Hosting.Diagnostics[2]
      Request finished HTTP/2 POST http://localhost:5001/Data.DataService/GetData application/grpc - - 200 - application/grpc 11313.8152ms
info: Microsoft.AspNetCore.Hosting.Diagnostics[1]
      Request starting HTTP/1.1 GET http://localhost:4999/api/DataService/GetData/2 - -
info: Microsoft.AspNetCore.Routing.EndpointMiddleware[0]
      Executing endpoint 'gRPC_REST.Controllers.DataServiceController.GetData (gRPC_REST)'
info: Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker[3]
      Route matched with {action = "GetData", controller = "DataService"}. Executing controller action with signature Microsoft.AspNetCore.Mvc.ActionResult`1[gRPC_REST.DataResponse] GetData(Int32) on controller gRPC_REST.Controllers.DataServiceController (gRPC_REST).
info: Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor[1]
      Executing OkObjectResult, writing value of type 'gRPC_REST.DataResponse'.
info: Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker[2]
      Executed action gRPC_REST.Controllers.DataServiceController.GetData (gRPC_REST) in 165.8012ms
info: Microsoft.AspNetCore.Routing.EndpointMiddleware[1]
      Executed endpoint 'gRPC_REST.Controllers.DataServiceController.GetData (gRPC_REST)'
info: Microsoft.AspNetCore.Hosting.Diagnostics[2]
      Request finished HTTP/1.1 GET http://localhost:4999/api/DataService/GetData/2 - - - 200 - application/json;+charset=utf-8 241.9674ms
info: Microsoft.AspNetCore.Hosting.Diagnostics[1]
      Request starting HTTP/1.1 GET http://localhost:4999/api/DataService/GetData/1000000 - -
info: Microsoft.AspNetCore.Routing.EndpointMiddleware[0]
      Executing endpoint 'gRPC_REST.Controllers.DataServiceController.GetData (gRPC_REST)'
info: Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker[3]
      Route matched with {action = "GetData", controller = "DataService"}. Executing controller action with signature Microsoft.AspNetCore.Mvc.ActionResult`1[gRPC_REST.DataResponse] GetData(Int32) on controller gRPC_REST.Controllers.DataServiceController (gRPC_REST).
info: Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor[1]
      Executing OkObjectResult, writing value of type 'gRPC_REST.DataResponse'.
info: Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker[2]
      Executed action gRPC_REST.Controllers.DataServiceController.GetData (gRPC_REST) in 18410.3456ms
info: Microsoft.AspNetCore.Routing.EndpointMiddleware[1]
      Executed endpoint 'gRPC_REST.Controllers.DataServiceController.GetData (gRPC_REST)'
info: Microsoft.AspNetCore.Hosting.Diagnostics[2]
      Request finished HTTP/1.1 GET http://localhost:4999/api/DataService/GetData/1000000 - - - 200 - application/json;+charset=utf-8 18423.4654ms
```

</details>

## Analysis
TL;DR

gRPC is a **lot** faster due to better deserialisation

### Discussion
On the face of it, from a client perspective, gRPC sends over 1 million records in 15s 
whereas REST takes 120s.  This is an order of magnitude difference!

However, examining the server logs tells a different story.  gRPC sends the data in 11.3s,
whereas REST takes 18.4s  REST is certainly slower, probably due to a slightly more verbose
wire format, but not an order of magnitude slower.

The underlying reason why REST is slower is probably due to (client side) deserialisation from
JSON to a JObject (or similar) to POCOs (Plain Old C# Objects).  There is probably a bit of reflection
in the mix, depending on the JSON deserialiser.

In contrast, gRPC will use its own deserialisation mechanism which is simply traversing the wire data.
