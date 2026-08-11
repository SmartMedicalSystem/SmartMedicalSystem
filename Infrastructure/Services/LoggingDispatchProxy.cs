using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Infrastructure.Services
{
    /// <summary>
    /// DispatchProxy that logs:
    /// - Method entry
    /// - Method exit
    /// - Execution duration
    /// - Exceptions
    ///
    /// Supports:
    /// - Synchronous methods
    /// - Task
    /// - Task<T>
    ///
    /// Sensitive method arguments are intentionally not logged.
    /// </summary>
    public class LoggingDispatchProxy<T> : DispatchProxy
        where T : class
    {
        private T? _decorated;

        private ILogger<T> _logger =
            NullLogger<T>.Instance;


        public void Configure(
            T decorated,
            ILogger<T>? logger)
        {
            _decorated =
                decorated
                ?? throw new ArgumentNullException(
                    nameof(decorated));

            _logger =
                logger
                ?? NullLogger<T>.Instance;
        }


        protected override object? Invoke(
            MethodInfo? targetMethod,
            object?[]? args)
        {
            if (targetMethod == null)
            {
                throw new ArgumentNullException(
                    nameof(targetMethod));
            }

            if (_decorated == null)
            {
                throw new InvalidOperationException(
                    "Logging proxy has not been configured.");
            }

            var methodName =
                targetMethod.Name;

            _logger.LogInformation(
                "Entering {Service}.{Method}",
                typeof(T).Name,
                methodName);

            var stopwatch =
                Stopwatch.StartNew();

            try
            {
                var result =
                    targetMethod.Invoke(
                        _decorated,
                        args);

                var returnType =
                    targetMethod.ReturnType;


                // =========================================
                // Task
                // =========================================

                if (returnType == typeof(Task))
                {
                    return InterceptAsync(
                        (Task)result!,
                        stopwatch,
                        methodName);
                }


                // =========================================
                // Task<T>
                // =========================================

                if (returnType.IsGenericType &&
                    returnType.GetGenericTypeDefinition()
                    == typeof(Task<>))
                {
                    var resultType =
                        returnType.GetGenericArguments()[0];

                    var method =
                        typeof(LoggingDispatchProxy<T>)
                            .GetMethod(
                                nameof(
                                    InterceptAsyncGeneric),
                                BindingFlags.NonPublic |
                                BindingFlags.Instance)!
                            .MakeGenericMethod(
                                resultType);

                    return method.Invoke(
                        this,
                        new object[]
                        {
                            result!,
                            stopwatch,
                            methodName
                        });
                }


                // =========================================
                // Synchronous method
                // =========================================

                stopwatch.Stop();

                _logger.LogInformation(
                    "Exiting {Service}.{Method} " +
                    "took {ElapsedMilliseconds} ms",
                    typeof(T).Name,
                    methodName,
                    stopwatch.ElapsedMilliseconds);

                return result;
            }
            catch (TargetInvocationException ex)
                when (ex.InnerException != null)
            {
                stopwatch.Stop();

                _logger.LogError(
                    ex.InnerException,
                    "Exception in {Service}.{Method}",
                    typeof(T).Name,
                    methodName);

                // Preserve original exception stack trace
                System.Runtime.ExceptionServices
                    .ExceptionDispatchInfo
                    .Capture(ex.InnerException)
                    .Throw();

                throw;
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


        // =========================================
        // Task
        // =========================================

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
                    "Exiting {Service}.{Method} " +
                    "took {ElapsedMilliseconds} ms",
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


        // =========================================
        // Task<T>
        // =========================================

        private async Task<TResult>
            InterceptAsyncGeneric<TResult>(
                Task<TResult> task,
                Stopwatch stopwatch,
                string methodName)
        {
            try
            {
                var result =
                    await task.ConfigureAwait(false);

                stopwatch.Stop();

                _logger.LogInformation(
                    "Exiting {Service}.{Method} " +
                    "took {ElapsedMilliseconds} ms",
                    typeof(T).Name,
                    methodName,
                    stopwatch.ElapsedMilliseconds);

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