using System.Reflection;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Infrastructure.Services
{
    /// <summary>
    /// DispatchProxy that logs method entry/exit, arguments, duration and exceptions for interface-based services.
    /// </summary>
    public class LoggingDispatchProxy<T> : DispatchProxy where T : class
    {
        private T? _decorated;
        private ILogger<T> _logger = NullLogger<T>.Instance;

        // Accept nullable logger and use a NullLogger fallback to avoid failures if logger isn't available
        public void Configure(T decorated, ILogger<T>? logger)
        {
            _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
            _logger = logger ?? NullLogger<T>.Instance;
        }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod is null) throw new ArgumentNullException(nameof(targetMethod));
            if (_decorated is null) throw new InvalidOperationException("Proxy not configured");

            var methodName = targetMethod.Name;
            try
            {
                _logger?.LogInformation("Entering {Service}.{Method} with args {@Args}", typeof(T).Name, methodName, args);

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                var result = targetMethod.Invoke(_decorated, args);

                // If method returns Task or Task<T>, we need to await it to log duration and exceptions
                if (result is Task task)
                {
                    var resultType = result.GetType();
                    if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Task<>))
                    {
                        // call generic interceptor that returns Task<TResult> via reflection
                        var innerType = resultType.GetGenericArguments()[0];
                        var methodInfo = typeof(LoggingDispatchProxy<T>).GetMethod(nameof(InterceptAsyncGeneric), BindingFlags.NonPublic | BindingFlags.Instance);
                        var genericMethod = methodInfo!.MakeGenericMethod(innerType);
                        return genericMethod.Invoke(this, new object[] { result, stopwatch, methodName })!;
                    }

                    return InterceptAsync(task, stopwatch, methodName);
                }

                stopwatch.Stop();
                _logger.LogInformation("Exiting {Service}.{Method} took {Elapsed}ms returned {@Result}", typeof(T).Name, methodName, stopwatch.Elapsed.TotalMilliseconds, result);
                return result;
            }
            catch (TargetInvocationException tie) when (tie.InnerException != null)
            {
                _logger?.LogError(tie.InnerException, "Exception in {Service}.{Method}", typeof(T).Name, methodName);
                throw tie.InnerException;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Exception in {Service}.{Method}", typeof(T).Name, methodName);
                throw;
            }
        }

        private async Task InterceptAsync(Task task, Stopwatch stopwatch, string methodName)
        {
            try
            {
                await task.ConfigureAwait(false);
                stopwatch.Stop();
                _logger.LogInformation("Exiting {Service}.{Method} took {Elapsed}ms (async)", typeof(T).Name, methodName, stopwatch.Elapsed.TotalMilliseconds);
                return;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Exception in {Service}.{Method} (async)", typeof(T).Name, methodName);
                throw;
            }
        }

        private async Task<TResult> InterceptAsyncGeneric<TResult>(Task<TResult> task, Stopwatch stopwatch, string methodName)
        {
            try
            {
                var result = await task.ConfigureAwait(false);
                stopwatch.Stop();
                _logger.LogInformation("Exiting {Service}.{Method} took {Elapsed}ms returned {@Result} (async)", typeof(T).Name, methodName, stopwatch.Elapsed.TotalMilliseconds, result);
                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Exception in {Service}.{Method} (async)", typeof(T).Name, methodName);
                throw;
            }
        }
    }
}
