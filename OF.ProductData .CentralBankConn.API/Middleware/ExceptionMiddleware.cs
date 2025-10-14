namespace OF.ProductData.CentralBankConn.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // continue the pipeline
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = _env.IsDevelopment()
                ? new ErrorResponse
                {
                    StatusCode = context.Response.StatusCode,
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                }
                : new ErrorResponse
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "An unexpected error occurred."
                };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = System.Text.Json.JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }
    }
}

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
}
