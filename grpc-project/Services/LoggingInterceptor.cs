using Grpc.Core;
using Grpc.Core.Interceptors;

namespace grpc_project.Services;

public class LoggingInterceptor : Interceptor
{
    private readonly ILogger<LoggingInterceptor> _logger;

    public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        _logger.LogInformation($"Starting call. Method: {context.Method}");
        try
        {
            var response = await continuation(request, context);
            _logger.LogInformation($"Completed call. Method: {context.Method}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Call failed. Method: {context.Method}");
            throw;
        }
    }

    public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        _logger.LogInformation($"Starting client streaming call. Method: {context.Method}");
        try
        {
            var response = await continuation(requestStream, context);
            _logger.LogInformation($"Completed client streaming call. Method: {context.Method}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Client streaming call failed. Method: {context.Method}");
            throw;
        }
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        _logger.LogInformation($"Starting server streaming call. Method: {context.Method}");
        try
        {
            await continuation(request, responseStream, context);
            _logger.LogInformation($"Completed server streaming call. Method: {context.Method}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Server streaming call failed. Method: {context.Method}");
            throw;
        }
    }

    public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        _logger.LogInformation($"Starting duplex streaming call. Method: {context.Method}");
        try
        {
            await continuation(requestStream, responseStream, context);
            _logger.LogInformation($"Completed duplex streaming call. Method: {context.Method}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Duplex streaming call failed. Method: {context.Method}");
            throw;
        }
    }
}