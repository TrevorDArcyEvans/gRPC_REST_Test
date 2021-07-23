# gRPC vs REST

Based on code from:

[Tutorial: Create a gRPC client and server in ASP.NET Core](https://docs.microsoft.com/en-gb/aspnet/core/tutorials/grpc/grpc-start?view=aspnetcore-5.0&tabs=visual-studio)

[Troubleshoot gRPC on .NET Core](https://docs.microsoft.com/en-gb/aspnet/core/grpc/troubleshoot?view=aspnetcore-5.0)

[Create Protobuf messages for .NET apps](https://docs.microsoft.com/en-us/aspnet/core/grpc/protobuf?view=aspnetcore-5.0)

## Results

### Client

<details>

```bash
$ ./gRPC_REST_Client

Testing gRPC...
  Warmup call completed
Requested:  1000000
  21571 ms

Testing REST...
  Warmup call completed
Requested:  1000000
  79277 ms

Press any key to exit...
```

</details>

### Server

<details>

```bash
$ export ASPNETCORE_ENVIRONMENT=Development
$ ./gRPC_REST 

info: Microsoft.Hosting.Lifetime[0]
      Now listening on: http://[::]:4999
info: Microsoft.Hosting.Lifetime[0]
      Now listening on: http://[::]:5001
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: /home/trevorde/dev/gRPC_REST_Test/gRPC_REST/bin/Debug/net5.0
info: Microsoft.AspNetCore.Hosting.Diagnostics[1]
      Request starting HTTP/2 POST http://localhost:5001/Data.DataService/GetData application/grpc -
info: Microsoft.AspNetCore.Routing.EndpointMiddleware[0]
      Executing endpoint 'gRPC - /Data.DataService/GetData'
info: Microsoft.AspNetCore.Routing.EndpointMiddleware[1]
      Executed endpoint 'gRPC - /Data.DataService/GetData'
info: Microsoft.AspNetCore.Hosting.Diagnostics[2]
      Request finished HTTP/2 POST http://localhost:5001/Data.DataService/GetData application/grpc - - 200 - application/grpc 79.3118ms
info: Microsoft.AspNetCore.Hosting.Diagnostics[1]
      Request starting HTTP/2 POST http://localhost:5001/Data.DataService/GetData application/grpc -
info: Microsoft.AspNetCore.Routing.EndpointMiddleware[0]
      Executing endpoint 'gRPC - /Data.DataService/GetData'
info: Microsoft.AspNetCore.Routing.EndpointMiddleware[1]
      Executed endpoint 'gRPC - /Data.DataService/GetData'
info: Microsoft.AspNetCore.Hosting.Diagnostics[2]
      Request finished HTTP/2 POST http://localhost:5001/Data.DataService/GetData application/grpc - - 200 - application/grpc 19848.7517ms
info: Microsoft.AspNetCore.Hosting.Diagnostics[1]
      Request starting HTTP/1.1 GET http://localhost:4999/api/DataService/GetData/2 - -
info: Microsoft.AspNetCore.Routing.EndpointMiddleware[0]
      Executing endpoint 'gRPC_REST.Controllers.DataServiceController.GetData (gRPC_REST)'
info: Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker[3]
      Route matched with {action = "GetData", controller = "DataService"}. Executing controller action with signature Microsoft.AspNetCore.Mvc.ActionResult`1[gRPC_REST.DataResponse] GetData(Int32) on controller gRPC_REST.Controllers.DataServiceController (gRPC_REST).
info: Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor[1]
      Executing OkObjectResult, writing value of type 'gRPC_REST.DataResponse'.
info: Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker[2]
      Executed action gRPC_REST.Controllers.DataServiceController.GetData (gRPC_REST) in 41.7748ms
info: Microsoft.AspNetCore.Routing.EndpointMiddleware[1]
      Executed endpoint 'gRPC_REST.Controllers.DataServiceController.GetData (gRPC_REST)'
info: Microsoft.AspNetCore.Hosting.Diagnostics[2]
      Request finished HTTP/1.1 GET http://localhost:4999/api/DataService/GetData/2 - - - 200 - application/json;+charset=utf-8 68.2427ms
info: Microsoft.AspNetCore.Hosting.Diagnostics[1]
      Request starting HTTP/1.1 GET http://localhost:4999/api/DataService/GetData/1000000 - -
info: Microsoft.AspNetCore.Routing.EndpointMiddleware[0]
      Executing endpoint 'gRPC_REST.Controllers.DataServiceController.GetData (gRPC_REST)'
info: Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker[3]
      Route matched with {action = "GetData", controller = "DataService"}. Executing controller action with signature Microsoft.AspNetCore.Mvc.ActionResult`1[gRPC_REST.DataResponse] GetData(Int32) on controller gRPC_REST.Controllers.DataServiceController (gRPC_REST).
info: Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor[1]
      Executing OkObjectResult, writing value of type 'gRPC_REST.DataResponse'.
info: Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker[2]
      Executed action gRPC_REST.Controllers.DataServiceController.GetData (gRPC_REST) in 23713.6396ms
info: Microsoft.AspNetCore.Routing.EndpointMiddleware[1]
      Executed endpoint 'gRPC_REST.Controllers.DataServiceController.GetData (gRPC_REST)'
info: Microsoft.AspNetCore.Hosting.Diagnostics[2]
      Request finished HTTP/1.1 GET http://localhost:4999/api/DataService/GetData/1000000 - - - 200 - application/json;+charset=utf-8 23714.4551ms
```

</details>

## Analysis
TL;DR

gRPC is a **lot** faster due to better deserialisation

### Discussion
On the face of it, from a client perspective, gRPC sends 1 million records in 20s 
whereas REST takes 80s.  This is a 4x difference in throughput!

However, examining the server logs tells a different story.  gRPC sends the data in about 20s,
whereas REST takes just under 24s  REST is certainly slower, probably due to a slightly more verbose
wire format, but not 4x slower.

The underlying reason why REST is slower is probably due to (client side) deserialisation from
JSON to a JObject (or similar) to POCOs (Plain Old C# Objects).  There is probably a bit of reflection
in the mix, depending on the JSON deserialiser.

In contrast, gRPC will use its own deserialisation mechanism which is simply traversing the wire data.
