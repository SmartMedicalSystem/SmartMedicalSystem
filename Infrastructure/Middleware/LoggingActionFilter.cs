using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Infrastructure.Middleware
{
    public class LoggingActionFilter : IAsyncActionFilter
    {
        private readonly ILogger<LoggingActionFilter> _logger;

        public LoggingActionFilter(ILogger<LoggingActionFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var stopwatch = Stopwatch.StartNew();

            var controller = context.RouteData.Values["controller"]?.ToString();
            var action = context.RouteData.Values["action"]?.ToString();
            var user = context.HttpContext.User?.Identity?.Name ?? context.HttpContext.User?.FindFirst("sub")?.Value ?? "anonymous";

            _logger.LogInformation("Entering controller {Controller} action {Action} by {User} with args {@Args}", controller, action, user, context.ActionArguments);

            var resultContext = await next();

            stopwatch.Stop();

            var statusCode = resultContext.HttpContext.Response?.StatusCode;
            object? resultValue = null;

            if (resultContext.Result is ObjectResult or)
            {
                resultValue = or.Value;
            }

            _logger.LogInformation("Exiting controller {Controller} action {Action} by {User} took {Elapsed}ms returned {StatusCode} {@Result}",
                controller, action, user, stopwatch.Elapsed.TotalMilliseconds, statusCode, resultValue);
        }
    }
}
