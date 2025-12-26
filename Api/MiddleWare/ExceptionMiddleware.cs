using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using Api.Models;
using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.MiddleWare
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate requestDelegate, IHostEnvironment hostEnvironment, ILogger<ExceptionMiddleware> logger)
        {
            _requestDelegate = requestDelegate;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _requestDelegate(context);
            }
            catch (ApiException ex)
            {
                _logger.LogWarning(ex, "API exception occurred: {Message}", ex.Message);

                var problemDetails = new ProblemDetails
                {
                    Detail = ex.Message,
                    Status = ex.StatusCode,
                    Title = ex.ErrorCode,
                    Instance = context.Request.Path,
                    Type = $"https://tools.ietf.org/html/rfc7231#section-6.{ex.StatusCode / 100}.{ex.StatusCode % 100}"
                };

                await WriteProblemDetailsAsync(context, problemDetails, ex.StatusCode);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation exception occurred");

                var problemDetails = new ProblemDetails
                {
                    Detail = "One or more validation errors occurred",
                    Status = 400,
                    Title = "Validation Error",
                    Instance = context.Request.Path,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                };

                await WriteProblemDetailsAsync(context, problemDetails, 400);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt");

                var problemDetails = new ProblemDetails
                {
                    Detail = _hostEnvironment.IsDevelopment() ? ex.Message : "You do not have permission to access this resource",
                    Status = 403,
                    Title = "Forbidden",
                    Instance = context.Request.Path,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
                };

                await WriteProblemDetailsAsync(context, problemDetails, 403);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);

                var problemDetails = new ProblemDetails
                {
                    Detail = _hostEnvironment.IsDevelopment()
                        ? $"{ex.Message}\n{ex.StackTrace}"
                        : "An internal server error occurred. Please try again later.",
                    Status = 500,
                    Title = "Internal Server Error",
                    Instance = context.Request.Path,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
                };

                await WriteProblemDetailsAsync(context, problemDetails, 500);
            }
        }

        private async Task WriteProblemDetailsAsync(HttpContext context, ProblemDetails problemDetails, int statusCode)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsJsonAsync(problemDetails, options);
        }
    }

}
