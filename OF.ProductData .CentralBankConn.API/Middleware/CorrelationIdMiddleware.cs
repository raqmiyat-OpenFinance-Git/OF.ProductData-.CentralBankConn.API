namespace OF.ProductData.CentralBankConn.API.Middleware;

// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationHeader = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string correlationId;

        // Check if correlation ID exists in request
        if (context.Request.Headers.TryGetValue(CorrelationHeader, out var existingId) && !string.IsNullOrWhiteSpace(existingId))
        {
            correlationId = existingId!;
        }
        else
        {
            // Generate a new one
            correlationId = Guid.NewGuid().ToString();
        }

        // Store in HttpContext.Items (can be used later)
        context.Items[CorrelationHeader] = correlationId;

        // Optionally add it to the response headers
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[CorrelationHeader] = correlationId;
            return Task.CompletedTask;
        });

        await _next(context);
    }
}
