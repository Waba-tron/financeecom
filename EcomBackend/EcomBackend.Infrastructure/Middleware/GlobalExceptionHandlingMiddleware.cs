using EcomBackend.Application.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace EcomBackend.Infrastructure.Middleware;

/// <summary>
/// Middleware for handling all unhandled exceptions globally.
/// </summary>
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception occurred at {Path}", context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse();

        switch (exception)
        {
            case ValidationFailedException valFailedEx:
                context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                response = new ErrorResponse
                {
                    Message = valFailedEx.Message,
                    ErrorCode = valFailedEx.ErrorCode,
                    StatusCode = context.Response.StatusCode,
                    ValidationErrors = valFailedEx.ValidationFailures
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToList() as IList<string>)
                };
                break;

            case AppException appEx:
                context.Response.StatusCode = appEx.StatusCode ?? (int)HttpStatusCode.BadRequest;
                response = new ErrorResponse
                {
                    Message = appEx.Message,
                    ErrorCode = appEx.ErrorCode,
                    StatusCode = context.Response.StatusCode
                };
                break;

            case ValidationException valEx:
                context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                response = new ErrorResponse
                {
                    Message = "One or more validation failures have occurred.",
                    ErrorCode = "VALIDATION_FAILED",
                    StatusCode = context.Response.StatusCode,
                    ValidationErrors = valEx.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToList() as IList<string>)
                };
                break;

            case KeyNotFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response = new ErrorResponse
                {
                    Message = "The requested resource was not found.",
                    ErrorCode = "RESOURCE_NOT_FOUND",
                    StatusCode = context.Response.StatusCode
                };
                break;

            case ArgumentException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new ErrorResponse
                {
                    Message = exception.Message,
                    ErrorCode = "INVALID_ARGUMENT",
                    StatusCode = context.Response.StatusCode
                };
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new ErrorResponse
                {
                    Message = "An internal server error has occurred.",
                    ErrorCode = "INTERNAL_SERVER_ERROR",
                    StatusCode = context.Response.StatusCode
                };
                break;
        }

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}

/// <summary>
/// Standardized error response format.
/// </summary>
public class ErrorResponse
{
    public string? Message { get; set; }
    public string? ErrorCode { get; set; }
    public int StatusCode { get; set; }
    public Dictionary<string, IList<string>>? ValidationErrors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
