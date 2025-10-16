using System.Threading.Channels;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Health.V1;
using grpc_project;

// Create a channel to the gRPC server
// Use the HTTP endpoint that the server is listening on
using var channel = GrpcChannel.ForAddress("http://localhost:5263");
var client = new Greeter.GreeterClient(channel);
var healthClient = new Health.HealthClient(channel);

// Test Unary RPC
Console.WriteLine("=== Testing Unary RPC ===");
try
{
    var reply = await client.SayHelloAsync(new HelloRequest { Name = "Unary World" });
    Console.WriteLine($"Unary Response: {reply.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Unary RPC failed: {ex.Message}");
}

// Test Server Streaming RPC
Console.WriteLine("\n=== Testing Server Streaming RPC ===");
try
{
    using var streamingCall = client.SayHelloStream(new HelloRequest { Name = "Streaming World", Count = 3 });
    
    while (await streamingCall.ResponseStream.MoveNext())
    {
        var streamReply = streamingCall.ResponseStream.Current;
        Console.WriteLine($"Stream Response: {streamReply.Message}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Server Streaming RPC failed: {ex.Message}");
}

// Test Client Streaming RPC
Console.WriteLine("\n=== Testing Client Streaming RPC ===");
try
{
    using var clientStreamingCall = client.SayHelloClientStream();
    
    // Send multiple requests
    var names = new[] { "Alice", "Bob", "Charlie" };
    foreach (var name in names)
    {
        await clientStreamingCall.RequestStream.WriteAsync(new HelloRequest { Name = name });
    }
    
    // Complete the client stream
    await clientStreamingCall.RequestStream.CompleteAsync();
    
    // Get the response
    var clientStreamingReply = await clientStreamingCall;
    Console.WriteLine($"Client Streaming Response: {clientStreamingReply.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Client Streaming RPC failed: {ex.Message}");
}

// Test Bidirectional Streaming RPC
Console.WriteLine("\n=== Testing Bidirectional Streaming RPC ===");
try
{
    using var bidiStreamingCall = client.SayHelloBidirectionalStream();
    
    // Create a channel to handle responses asynchronously
    var responseChannel = Channel.CreateBounded<string>(new BoundedChannelOptions(2)
    {
        FullMode = BoundedChannelFullMode.DropOldest
    });
    
    // Start a task to read responses
    var responseTask = Task.Run(async () =>
    {
        try
        {
            while (await bidiStreamingCall.ResponseStream.MoveNext())
            {
                var bidiReply = bidiStreamingCall.ResponseStream.Current;
                await responseChannel.Writer.WriteAsync($"Bidirectional Response: {bidiReply.Message}");
            }
        }
        catch (RpcException ex)
        {
            await responseChannel.Writer.WriteAsync($"Bidirectional streaming error: {ex.Status}");
        }
    });
    
    // Send multiple requests
    var bidiNames = new[] { "Dave", "Eve", "Frank" };
    foreach (var name in bidiNames)
    {
        await bidiStreamingCall.RequestStream.WriteAsync(new HelloRequest { Name = name });
        await Task.Delay(1000); // Wait a bit between requests
    }
    
    // Complete the client stream
    await bidiStreamingCall.RequestStream.CompleteAsync();
    
    // Read responses
    await responseTask;
    
    // Print any remaining responses
    responseChannel.Writer.Complete();
    await foreach (var response in responseChannel.Reader.ReadAllAsync())
    {
        Console.WriteLine(response);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Bidirectional Streaming RPC failed: {ex.Message}");
}

// Test GetUser RPC
Console.WriteLine("\n=== Testing GetUser RPC ===");
try
{
    var userReply = await client.GetUserAsync(new UserRequest { UserId = "1" });
    Console.WriteLine($"User Response: {userReply.Name} ({userReply.Email}), Age: {userReply.Age}");
    Console.WriteLine($"Hobbies: {string.Join(", ", userReply.Hobbies)}");
}
catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
{
    Console.WriteLine($"User not found: {ex.Status.Detail}");
}
catch (Exception ex)
{
    Console.WriteLine($"GetUser RPC failed: {ex.Message}");
}

// Test Health Check
Console.WriteLine("\n=== Testing Health Check ===");
try
{
    var healthReply = await healthClient.CheckAsync(new HealthCheckRequest { Service = "grpc_project.Greeter" });
    Console.WriteLine($"Health Check Status: {healthReply.Status}");
}
catch (Exception ex)
{
    Console.WriteLine($"Health Check failed: {ex.Message}");
}

Console.WriteLine("\n=== All tests completed ===");