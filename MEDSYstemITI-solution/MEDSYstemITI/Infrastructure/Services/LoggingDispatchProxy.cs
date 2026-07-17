using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Infrastructure.Services
{
    /// <summary>
    /// DispatchProxy that logs method entry/exit, arguments, duration and exceptions.
    /// Supports synchronous methods, Task and Task&lt;T&gt;.
    /// </summary>
    public class LoggingDispatchProxy<T> : DispatchProxy where T : class
    {
        private T? _decorated;
        private ILogger<T> _logger = NullLogger<T>.Instance;

        public void Configure(T decorated, ILogger<T>? logger)
        {
            _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
            _logger = logger ?? NullLogger<T>.Instance;
        }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod == null)
                throw new ArgumentNullException(nameof(targetMethod));

            if (_decorated == null)
                throw new InvalidOperationException("Proxy not configured.");

            string methodName = targetMethod.Name;

            _logger.LogInformation(
                "Entering {Service}.{Method} with args {@Args}",
                typeof(T).Name,
                methodName,
                args);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                object? result = targetMethod.Invoke(_decorated, args);

                Type returnType = targetMethod.ReturnType;

                // ---------------- Task ----------------

                if (returnType == typeof(Task))
                {
                    return InterceptAsync(
                        (Task)result!,
                        stopwatch,
                        methodName);
                }

                // ---------------- Task<T> ----------------

                if (returnType.IsGenericType &&
                    returnType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    Type resultType = returnType.GetGenericArguments()[0];

                    MethodInfo method =
                        typeof(LoggingDispatchProxy<T>)
                            .GetMethod(
                                nameof(InterceptAsyncGeneric),
                                BindingFlags.NonPublic | BindingFlags.Instance)!
                            .MakeGenericMethod(resultType);

                    return method.Invoke(
                        this,
                        new object[]
                        {
                            result!,
                            stopwatch,
                            methodName
                        });
                }

                // ---------------- Sync ----------------

                stopwatch.Stop();

                _logger.LogInformation(
                    "Exiting {Service}.{Method} took {Elapsed} ms returned {@Result}",
                    typeof(T).Name,
                    methodName,
                    stopwatch.ElapsedMilliseconds,
                    result);

                return result;
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                stopwatch.Stop();

                _logger.LogError(
                    ex.InnerException,
                    "Exception in {Service}.{Method}",
                    typeof(T).Name,
                    methodName);

                throw ex.InnerException;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(
                    ex,
                    "Exception in {Service}.{Method}",
                    typeof(T).Name,
                    methodName);

                throw;
            }
        }

        private async Task InterceptAsync(
            Task task,
            Stopwatch stopwatch,
            string methodName)
        {
            try
            {
                await task.ConfigureAwait(false);

                stopwatch.Stop();

                _logger.LogInformation(
                    "Exiting {Service}.{Method} took {Elapsed} ms",
                    typeof(T).Name,
                    methodName,
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(
                    ex,
                    "Exception in {Service}.{Method}",
                    typeof(T).Name,
                    methodName);

                throw;
            }
        }

        private async Task<TResult> InterceptAsyncGeneric<TResult>(
            Task<TResult> task,
            Stopwatch stopwatch,
            string methodName)
        {
            try
            {
                TResult result = await task.ConfigureAwait(false);

                stopwatch.Stop();

                _logger.LogInformation(
                    "Exiting {Service}.{Method} took {Elapsed} ms returned {@Result}",
                    typeof(T).Name,
                    methodName,
                    stopwatch.ElapsedMilliseconds,
                    result);

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(
                    ex,
                    "Exception in {Service}.{Method}",
                    typeof(T).Name,
                    methodName);

                throw;
            }
        }
    }
}