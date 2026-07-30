using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;


public class LogSensitiveActionFilter : IAsyncActionFilter
{
    private readonly ILogger<LogSensitiveActionFilter> _logger;

    public LogSensitiveActionFilter(
        ILogger<LogSensitiveActionFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var actionName =
            context.ActionDescriptor.DisplayName;

        var userId =
            context.HttpContext.User.FindFirstValue(
                ClaimTypes.NameIdentifier);

        var userName =
            context.HttpContext.User.Identity?.Name;

        var ipAddress =
            context.HttpContext.Connection.RemoteIpAddress?
                .ToString();

        _logger.LogInformation(
            "Sensitive action started. " +
            "UserId: {UserId}, " +
            "UserName: {UserName}, " +
            "Action: {ActionName}, " +
            "IP: {IpAddress}",
            userId,
            userName,
            actionName,
            ipAddress);

        var executedContext = await next();

        if (executedContext.Exception is not null &&
            !executedContext.ExceptionHandled)
        {
            _logger.LogError(
                executedContext.Exception,
                "Sensitive action failed. " +
                "UserId: {UserId}, " +
                "Action: {ActionName}",
                userId,
                actionName);
        }
        else
        {
            _logger.LogInformation(
                "Sensitive action completed successfully. " +
                "UserId: {UserId}, " +
                "Action: {ActionName}",
                userId,
                actionName);
        }
    }
}