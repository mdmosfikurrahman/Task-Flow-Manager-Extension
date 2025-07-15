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
            var interfaceTypes = implementationType.GetInterfaces()
                .Where(i => i.Namespace == "Task_Flow_Manager_Extension.Services");

            foreach (var interfaceType in interfaceTypes)
            {
                services.AddScoped(interfaceType, implementationType);
            }
        }

        services.AddOptions()
            .Configure<JsonSerializerOptions>(options => { options.PropertyNameCaseInsensitive = true; });

        services.AddHttpClient();
        services.AddHttpContextAccessor();
    }

}