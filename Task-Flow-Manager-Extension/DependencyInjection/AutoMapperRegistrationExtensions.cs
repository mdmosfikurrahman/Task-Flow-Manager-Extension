using AutoMapper;
using Task_Flow_Manager_Extension.Mappers;

namespace Task_Flow_Manager_Extension.DependencyInjection;

public static class AutoMapperRegistrationExtensions
{
    public static void RegisterAutoMapperProfiles(this IServiceCollection services)
    {
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        MapperExtensions.Configure(services.BuildServiceProvider().CreateScope().ServiceProvider
            .GetRequiredService<IMapper>());
    }
}