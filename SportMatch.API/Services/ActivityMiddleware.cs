namespace SportMatch.API.Services;

public class ActivityMiddleware
{
    private readonly RequestDelegate _next;

    public ActivityMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ILiveMetricsService metrics)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var username = context.User.Identity.Name;
            if (!string.IsNullOrEmpty(username))
            {
                metrics.Heartbeat(username);
            }
        }

        await _next(context);
    }
}