# Advanced gRPC Project

This project demonstrates the full potential of gRPC with implementations of all four types of RPC calls, health checks, interceptors, and more.

## Features

1. **Unary RPC** - Traditional request-response
2. **Server Streaming RPC** - Server sends multiple responses
3. **Client Streaming RPC** - Client sends multiple requests
4. **Bidirectional Streaming RPC** - Both client and server stream
5. **Health Checks** - Standard gRPC health checking
6. **Interceptors** - Logging and monitoring
7. **Error Handling** - Proper gRPC error codes
8. **Timestamp Support** - Using Protocol Buffer Well-Known Types

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

## Getting Started

1. Clone this repository
2. Navigate to the project directory
3. Run the application:
   ```bash
   dotnet run --project grpc-project/grpc-project.csproj
   ```

The server will start on `http://localhost:5263`.

## Service Methods

### Unary RPC
```
rpc SayHello (HelloRequest) returns (HelloReply);
```
Simple request-response method.

### Server Streaming RPC
```
rpc SayHelloStream (HelloRequest) returns (stream HelloReply);
```
Server sends multiple responses to a single client request.

### Client Streaming RPC
```
rpc SayHelloClientStream (stream HelloRequest) returns (HelloReply);
```
Client sends multiple requests, server responds once.

### Bidirectional Streaming RPC
```
rpc SayHelloBidirectionalStream (stream HelloRequest) returns (stream HelloReply);
```
Both client and server can send multiple messages.

### User Service
```
rpc GetUser (UserRequest) returns (UserResponse);
```
Demonstrates working with complex data structures.

### Health Check
```
rpc Check(HealthCheckRequest) returns (HealthCheckResponse);
rpc Watch(HealthCheckRequest) returns (stream HealthCheckResponse);
```
Standard gRPC health checking service.

## Running the Application

To run the server:
```bash
dotnet run --project grpc-project/grpc-project.csproj
```

To run the client (in a separate terminal):
```bash
dotnet run --project grpc-client/grpc-client.csproj
```

Or use the provided batch script on Windows:
```bash
test-grpc.bat
```

## Testing

You can test the gRPC services using tools like:
- [BloomRPC](https://github.com/bloomrpc/bloomrpc)
- [gRPCurl](https://github.com/fullstorydev/grpcurl)
- [Postman](https://www.postman.com/)

Example gRPCurl commands:
```bash
# Unary call
grpcurl -plaintext -d '{"name": "World"}' localhost:5263 greet.Greeter/SayHello

# Server streaming call
grpcurl -plaintext -d '{"name": "World", "count": 3}' localhost:5263 greet.Greeter/SayHelloStream
```

## Project Structure

```
├── Protos/
│   ├── greet.proto         # Main service definitions
│   └── health.proto        # Standard health checking
├── Services/
│   ├── GreeterService.cs   # Implementation of all service methods
│   ├── HealthService.cs    # Health checking implementation
│   └── LoggingInterceptor.cs # Logging interceptor
├── Program.cs              # Application entry point
├── appsettings.json        # Configuration
└── grpc-project.csproj     # Server project file
├── grpc-client/
│   ├── Program.cs          # Client implementation
│   └── grpc-client.csproj  # Client project file
├── test-grpc.bat           # Windows batch script to run server and client
└── grpc-project.sln        # Solution file
```

## Key Concepts Demonstrated

1. **All Four RPC Types**: Shows how to implement unary, server streaming, client streaming, and bidirectional streaming RPCs.

2. **Proper Error Handling**: Uses appropriate gRPC status codes and exceptions.

3. **Interceptors**: Implements logging interceptors for cross-cutting concerns.

4. **Health Checks**: Implements the standard gRPC health checking protocol.

5. **Protocol Buffer Best Practices**: Uses well-known types like Timestamp.

6. **Configuration**: Demonstrates proper configuration for gRPC services.

7. **Dependency Injection**: Uses ASP.NET Core's built-in DI container.

## Extending the Project

To add new service methods:
1. Update the `.proto` file with new message types and service methods
2. Regenerate the gRPC classes (happens automatically in .NET)
3. Implement the new methods in the service class
4. Add appropriate logging and error handling

## Learn More

- [gRPC Documentation](https://grpc.io/docs/)
- [Protocol Buffers](https://developers.google.com/protocol-buffers)
- [ASP.NET Core gRPC](https://docs.microsoft.com/en-us/aspnet/core/grpc/)