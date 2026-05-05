using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Clinicks.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Clinicks.API.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
                _logger.LogError(ex, "Ocurrió una excepción no controlada.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = exception switch
            {
                NotFoundException => (int)HttpStatusCode.NotFound,
                ConflictException => (int)HttpStatusCode.Conflict,
                ValidationException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = exception is AppException ? exception.Message : "Ocurrió un error interno en el servidor."
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
