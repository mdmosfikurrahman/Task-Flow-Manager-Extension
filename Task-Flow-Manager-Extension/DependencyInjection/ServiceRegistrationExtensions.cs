using System.Text.Json;

namespace Task_Flow_Manager_Extension.DependencyInjection;

public static class ServiceRegistrationExtensions
{
    public static void RegisterServices(this IServiceCollection services)
    {
        var assembly = typeof(ServiceRegistrationExtensions).Assembly;

        var serviceTypes = assembly.GetTypes()
            .Where(type => type is
            {
                IsClass: true,
                IsAbstract: false,
                Namespace: "Task_Flow_Manager_Extension.Services.Impl"
            });

        foreach (var implementationType in serviceTypes)
        {
            var interfaceType = implementationType.GetInterfaces()
                .FirstOrDefault(i => i.Name == $"I{implementationType.Name.Replace("Impl", "")}");

            if (interfaceType != null)
            {
                services.AddScoped(interfaceType, implementationType);
            }
        }

        services.AddOptions()
            .Configure<JsonSerializerOptions>(options => { options.PropertyNameCaseInsensitive = true; });

        // Register HTTP client
        services.AddHttpClient();
        services.AddHttpContextAccessor();
    }
}