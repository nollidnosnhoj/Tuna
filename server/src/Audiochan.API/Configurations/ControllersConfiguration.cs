using System.Text.Json;
using System.Text.Json.Serialization;
using Audiochan.API.Filters;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.API.Configurations
{
    public static class ControllersConfiguration
    {
        public static IServiceCollection ConfigureControllers(this IServiceCollection services)
        {
            services
                .AddControllers(configuration =>
                {
                    configuration.Filters.Add(new ProducesAttribute("application/json"));
                    configuration.Filters.Add<ValidationFilter>();
                })
                .AddJsonOptions(configuration =>
                {
                    configuration.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    configuration.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .AddFluentValidation(configuration =>
                    configuration.ValidatorFactoryType = typeof(HttpContextServiceProviderValidatorFactory));

            return services;
        }
    }
}