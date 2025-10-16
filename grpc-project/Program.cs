using grpc_project.Services;
using Grpc.Health.V1;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc(options =>
{
    // Configure gRPC settings
    options.EnableDetailedErrors = true;
    options.MaxReceiveMessageSize = 1024 * 1024; // 1 MB
    options.MaxSendMessageSize = 1024 * 1024; // 1 MB
});

// Add our custom interceptor
builder.Services.AddSingleton<LoggingInterceptor>();

// Add health checks
builder.Services.AddGrpcHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Note: UseGrpcWeb is not needed for this implementation

// Map gRPC services
app.MapGrpcService<GreeterService>();
app.MapGrpcService<HealthService>();

app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

// Add a simple status endpoint
app.MapGet("/status", () => Results.Ok(new { status = "running", timestamp = DateTime.UtcNow }));

app.Run();