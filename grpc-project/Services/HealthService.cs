using Grpc.Core;
using Grpc.Health.V1;

namespace grpc_project.Services;

public class HealthService : Health.HealthBase
{
    private readonly ILogger<HealthService> _logger;
    private readonly Dictionary<string, HealthCheckResponse.Types.ServingStatus> _serviceStatuses;

    public HealthService(ILogger<HealthService> logger)
    {
        _logger = logger;
        _serviceStatuses = new Dictionary<string, HealthCheckResponse.Types.ServingStatus>
        {
            { "", HealthCheckResponse.Types.ServingStatus.Serving }, // Overall service status
            { "grpc_project.Greeter", HealthCheckResponse.Types.ServingStatus.Serving } // Specific service status
        };
    }

    public override Task<HealthCheckResponse> Check(HealthCheckRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Health check requested for service: {Service}", request.Service);

        var service = request.Service;
        if (!_serviceStatuses.ContainsKey(service))
        {
            // If we don't know about this service, return SERVICE_UNKNOWN
            return Task.FromResult(new HealthCheckResponse
            {
                Status = HealthCheckResponse.Types.ServingStatus.ServiceUnknown
            });
        }

        return Task.FromResult(new HealthCheckResponse
        {
            Status = _serviceStatuses[service]
        });
    }

    public override async Task Watch(HealthCheckRequest request, IServerStreamWriter<HealthCheckResponse> responseStream, ServerCallContext context)
    {
        _logger.LogInformation("Health watch requested for service: {Service}", request.Service);

        var service = request.Service;
        if (!_serviceStatuses.ContainsKey(service))
        {
            // If we don't know about this service, send SERVICE_UNKNOWN and complete
            await responseStream.WriteAsync(new HealthCheckResponse
            {
                Status = HealthCheckResponse.Types.ServingStatus.ServiceUnknown
            });
            return;
        }

        // Send current status
        await responseStream.WriteAsync(new HealthCheckResponse
        {
            Status = _serviceStatuses[service]
        });

        // For demo purposes, we'll send updates periodically
        // In a real application, this would be tied to actual service health monitoring
        try
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                await Task.Delay(30000, context.CancellationToken); // Send update every 30 seconds
                
                if (!context.CancellationToken.IsCancellationRequested)
                {
                    await responseStream.WriteAsync(new HealthCheckResponse
                    {
                        Status = _serviceStatuses[service]
                    });
                }
            }
        }
        catch (TaskCanceledException)
        {
            // Client disconnected, which is normal
            _logger.LogInformation("Health watch client disconnected");
        }
    }
}