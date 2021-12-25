using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Audiochan.API.Models;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Audiochan.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly JsonSerializerOptions _jsonOptions;


        public ExceptionHandlingMiddleware(RequestDelegate next
            , IWebHostEnvironment env
            , ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _env = env;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }

        private async Task HandleException(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            int code;
            object response;

            switch (ex)
            {
                case ValidationException vex:
                    code = StatusCodes.Status422UnprocessableEntity;
                    var errors = vex.Errors
                        .GroupBy(x => x.PropertyName, x => x.ErrorMessage)
                        .ToDictionary(x => x.Key, x => x.ToArray());
                    response = new ValidationErrorApiResponse("Invalid request.", errors);
                    break;
                default:
                    code = StatusCodes.Status500InternalServerError;
                    var message = _env.IsDevelopment()
                        ? ex.Message
                        : "An unexpected error has occurred.";
                    response = new ErrorApiResponse(message, null);
                    break;
            }

            context.Response.StatusCode = code;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, _jsonOptions));
        }
    }
}