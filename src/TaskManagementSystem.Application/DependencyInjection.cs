using Microsoft.Extensions.DependencyInjection;
using TaskManagementSystem.Application.Services;

namespace TaskManagementSystem.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ITaskService, TaskService>();

            return services;
        }
    }
}
