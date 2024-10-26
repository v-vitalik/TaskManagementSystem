using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagementSystem.Application.Options;
using TaskManagementSystem.Application.Services;

namespace TaskManagementSystem.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITaskService, TaskService>();
            var rabbitMqOptions = new RabbitMqOptions();
            configuration.GetSection(RabbitMqOptions.SectionName).Bind(rabbitMqOptions);
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMqOptions.Host, "/", h =>
                    {
                        h.Username(rabbitMqOptions.Username);
                        h.Password(rabbitMqOptions.Password);
                    });
                });
            });
            services.AddSingleton<IServiceBusHandler, ServiceBusHandler>();

            return services;
        }
    }
}
