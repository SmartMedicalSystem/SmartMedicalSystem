using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using Application.Common;
using Application.Common.Models;

// Global exception middleware that returns a consistent ErrorResponse JSON object

namespace MEDSYstemITI.Middleware
{
    /// <summary>
    /// Central place to translate exceptions thrown by the Application layer
    /// (NotFoundException, ArgumentException/ArgumentNullException for business
    /// rule violations) into consistent HTTP responses, so controllers don't need
    /// try/catch blocks around every service call.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Default response
            var traceId = context.TraceIdentifier ?? Guid.NewGuid().ToString();

            ErrorResponse response;
            int httpStatus;

            if (exception is AppException appEx)
            {
                httpStatus = appEx.StatusCode;

                response = new ErrorResponse
                {
                    StatusCode = appEx.StatusCode,
                    Message = appEx.Message,
                    ErrorCode = appEx.ErrorCode,
                    TraceId = traceId,
                    Errors = appEx.Errors is null ? new List<ErrorDetail>() : new List<ErrorDetail>(appEx.Errors)
                };

                // Expected application exceptions are less severe
                _logger.LogWarning(exception, "Handled application exception. TraceId: {TraceId}, ErrorCode: {ErrorCode}, Path: {Path}, Method: {Method}",
                    traceId, appEx.ErrorCode, context.Request.Path, context.Request.Method);
            }
            else if (exception is ArgumentException argEx)
            {
                httpStatus = (int)HttpStatusCode.BadRequest;
                response = new ErrorResponse
                {
                    StatusCode = httpStatus,
                    Message = argEx.Message,
                    ErrorCode = "BAD_REQUEST",
                    TraceId = traceId
                };

                _logger.LogWarning(exception, "Bad request. TraceId: {TraceId}, Path: {Path}, Method: {Method}", traceId, context.Request.Path, context.Request.Method);
            }
            else
            {
                httpStatus = (int)HttpStatusCode.InternalServerError;
                response = new ErrorResponse
                {
                    StatusCode = httpStatus,
                    Message = "An unexpected error occurred.",
                    ErrorCode = "INTERNAL_SERVER_ERROR",
                    TraceId = traceId
                };

                _logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}, Path: {Path}, Method: {Method}",
                    traceId, context.Request.Path, context.Request.Method);
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = httpStatus;

            var payload = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(payload);
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
