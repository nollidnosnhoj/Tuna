using System;
using System.Text.Json;
using System.Threading.Tasks;
using Audiochan.API.Models;
using Audiochan.Core.Common.Exceptions;
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
            
            ErrorApiResponse response;

            switch (ex)
            {
                case ValidationException vex:
                    response = ErrorApiResponse.Invalid(vex.Errors);
                    break;
                case UnauthorizedException:
                    response = ErrorApiResponse.Unauthorized();
                    break;
                case ForbiddenAccessException:
                    response = ErrorApiResponse.Forbidden();
                    break;
                default:
                    var message = _env.IsDevelopment()
                        ? ex.Message
                        : "An unexpected error has occurred.";
                    response = new ErrorApiResponse(StatusCodes.Status500InternalServerError, message, null);
                    break;
            }

            context.Response.StatusCode = response.Code;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, _jsonOptions));
        }
    }
}