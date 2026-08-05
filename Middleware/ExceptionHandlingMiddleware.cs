using System.Net;
using System.Text.Json;
using DotnetCRUD.Exceptions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DotnetCRUD.Middleware;

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
        var traceId = context.TraceIdentifier;

        var (statusCode, code, message, details) = Resolve(exception);

        if (statusCode >= 500)
        {
            _logger.LogError(exception, "Unhandled error {ErrorCode} on {Method} {Path}, TraceId {TraceId}",
                code, context.Request.Method, context.Request.Path, traceId);
        }
        else
        {
            _logger.LogWarning("Request {Method} {Path} failed with {StatusCode} {ErrorCode}: {Message}, TraceId {TraceId}",
                context.Request.Method, context.Request.Path, statusCode, code, message, traceId);
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var payload = new Dictionary<string, object?>
        {
            ["error"] = new Dictionary<string, object?>
            {
                ["code"] = code,
                ["message"] = message,
                ["traceId"] = traceId,
                ["details"] = details
            }
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }

    private static (int StatusCode, string Code, string Message, object? Details) Resolve(Exception exception)
    {
        switch (exception)
        {
            case ApiException apiException:
                return (apiException.StatusCode, apiException.Code, apiException.Message, null);

            case ValidationException validationException:
                return (
                    StatusCodes.Status400BadRequest,
                    "VALIDATION_ERROR",
                    "Validasi request gagal.",
                    validationException.Errors.Select(error => new
                    {
                        field = error.PropertyName,
                        message = error.ErrorMessage
                    })
                );

            case DbUpdateConcurrencyException:
                return (
                    StatusCodes.Status409Conflict,
                    "CONCURRENCY_CONFLICT",
                    "Data sudah diubah pihak lain. Silakan muat ulang lalu coba lagi.",
                    null
                );

            case DbUpdateException { InnerException: PostgresException postgresException }:
                return ResolvePostgres(postgresException);

            case PostgresException postgresException:
                return ResolvePostgres(postgresException);

            default:
                return (
                    StatusCodes.Status500InternalServerError,
                    "INTERNAL_ERROR",
                    "Terjadi kesalahan internal pada server.",
                    null
                );
        }
    }

    private static (int StatusCode, string Code, string Message, object? Details) ResolvePostgres(PostgresException ex)
    {
        return ex.SqlState switch
        {
            "23505" => (
                StatusCodes.Status409Conflict,
                "DUPLICATE_RESOURCE",
                "Data yang sama sudah terdaftar.",
                null
            ),
            "23503" => (
                StatusCodes.Status409Conflict,
                "FOREIGN_KEY_CONFLICT",
                "Data masih digunakan oleh data lain sehingga tidak bisa diproses.",
                null
            ),
            "40001" or "40P01" => (
                StatusCodes.Status409Conflict,
                "CONCURRENCY_CONFLICT",
                "Terjadi bentrok saat memproses data bersamaan. Silakan coba lagi.",
                null
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                "DATABASE_ERROR",
                "Terjadi kesalahan pada database.",
                null
            )
        };
    }
}
