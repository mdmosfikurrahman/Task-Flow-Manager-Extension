namespace Task_Flow_Manager_Extension.DependencyInjection;

public static class RepositoryRegistrationExtensions
{
    public static void RegisterRepositories(this IServiceCollection services)
    {
        var assembly = typeof(RepositoryRegistrationExtensions).Assembly;

        var repositoryTypes = assembly.GetTypes()
            .Where(type => type is
            {
                IsClass: true,
                IsAbstract: false,
                Namespace: "Task_Flow_Manager_Extension.Repositories.Impl",
                ContainsGenericParameters: false
            });

        foreach (var implementationType in repositoryTypes)
        {
            var interfaceType = implementationType.GetInterfaces()
                .FirstOrDefault(i => i.Name == $"I{implementationType.Name.Replace("Impl", "")}");

            if (interfaceType != null)
            {
                services.AddScoped(interfaceType, implementationType);
            }
        }
    }
}