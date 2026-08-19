namespace Web.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger
    )
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BadHttpRequestException ex)
        {
            _logger.LogWarning(ex, "Bad request");

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                errorCode = "BadRequest",
                error = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Internal server error");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new
            {
                errorCode = "InternalServerError",
                error = "Internal server error"
            });
        }
    }
}