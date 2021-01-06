using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;

namespace Audiochan.Web.Middlewares
{
    // ReSharper disable once ClassNeverInstantiated.Global
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

            var message = _env.IsDevelopment()
                ? ex.Message
                : "An unknown error has occurred. Please contact the administrators.";

            var response = new ErrorViewModel("Unknown Error", message, null);

            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response
                .WriteAsync(JsonSerializer.Serialize(response, _jsonOptions));
        }
    }
}
