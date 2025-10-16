using Grpc.Core;
using grpc_project;
using Google.Protobuf.WellKnownTypes;

namespace Services;

public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;
    private readonly Dictionary<string, UserResponse> _users;

    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
        
        // Initialize with some sample users
        _users = new Dictionary<string, UserResponse>
        {
            {
                "1", new UserResponse
                {
                    UserId = "1",
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    Age = 30,
                    Hobbies = { "Reading", "Swimming", "Coding" },
                    CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow)
                }
            },
            {
                "2", new UserResponse
                {
                    UserId = "2",
                    Name = "Jane Smith",
                    Email = "jane.smith@example.com",
                    Age = 25,
                    Hobbies = { "Painting", "Dancing", "Cooking" },
                    CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow)
                }
            }
        };
    }

    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Saying hello to {Name}", request.Name);
        
        return Task.FromResult(new HelloReply
        {
            Message = $"Hello {request.Name}",
            Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
        });
    }
    
    public override async Task SayHelloStream(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
    {
        _logger.LogInformation("Streaming greetings to {Name}", request.Name);
        
        var count = request.Count > 0 ? request.Count : 5;
        
        for (int i = 0; i < count && !context.CancellationToken.IsCancellationRequested; i++)
        {
            var reply = new HelloReply
            {
                Message = $"Hello {request.Name} - Message #{i + 1}",
                Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
            };
            
            await responseStream.WriteAsync(reply);
            
            // Add a small delay to demonstrate streaming
            await Task.Delay(500, context.CancellationToken);
        }
    }
    
    public override async Task<HelloReply> SayHelloClientStream(IAsyncStreamReader<HelloRequest> requestStream, ServerCallContext context)
    {
        _logger.LogInformation("Receiving stream of names");
        
        var names = new List<string>();
        
        await foreach (var request in requestStream.ReadAllAsync())
        {
            names.Add(request.Name);
            _logger.LogInformation("Received name: {Name}", request.Name);
        }
        
        var message = $"Hello to all of you: {string.Join(", ", names)}";
        
        return new HelloReply
        {
            Message = message,
            Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
        };
    }
    
    public override async Task SayHelloBidirectionalStream(IAsyncStreamReader<HelloRequest> requestStream, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
    {
        _logger.LogInformation("Starting bidirectional streaming");
        
        await foreach (var request in requestStream.ReadAllAsync())
        {
            var reply = new HelloReply
            {
                Message = $"Hello {request.Name} - Thanks for sending this at {DateTime.UtcNow}",
                Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
            };
            
            await responseStream.WriteAsync(reply);
        }
        
        _logger.LogInformation("Bidirectional streaming completed");
    }
    
    public override Task<UserResponse> GetUser(UserRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Getting user with ID: {UserId}", request.UserId);
        
        if (_users.TryGetValue(request.UserId, out var user))
        {
            return Task.FromResult(user);
        }
        
        // If user not found, throw appropriate gRPC exception
        throw new RpcException(new Status(StatusCode.NotFound, $"User with ID {request.UserId} not found"));
    }
}
