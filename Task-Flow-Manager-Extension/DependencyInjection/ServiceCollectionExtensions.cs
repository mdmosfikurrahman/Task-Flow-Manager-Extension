namespace Task_Flow_Manager_Extension.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void RegisterApplicationServices(this IServiceCollection services)
    {
        services.RegisterServices();
        services.RegisterRepositories();
        services.RegisterAutoMapperProfiles();
    }
}