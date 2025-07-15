using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Task_Flow_Manager_Extension.Data;
using Task_Flow_Manager_Extension.Infrastructure.Config;

namespace Task_Flow_Manager_Extension.DependencyInjection;

public static class StartupConfigurationExtensions
{
    public static void ConfigureProjectSettings(this WebApplicationBuilder builder)
    {
        builder.ConfigureDatabase();
        builder.ConfigureApiVersioning();
        builder.ConfigureMvcAndSwagger();
        builder.ConfigureGraphQl();
        builder.ConfigureAppSettings();
        builder.ConfigureRedis();
        builder.Services.RegisterApplicationServices();
        builder.Services.AddGrpc();

    }

    private static void ConfigureDatabase(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("TaskFlowManagerExtensionDb");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'TaskFlowManagerExtensionDb' is not configured.");
        }

        builder.Services.AddDbContext<TaskFlowManagerExtensionDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
    }

    private static void ConfigureApiVersioning(this WebApplicationBuilder builder)
    {
        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        });
    }

    private static void ConfigureMvcAndSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
            });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHttpContextAccessor();
    }

    private static void ConfigureGraphQl(this WebApplicationBuilder builder)
    {
        var assembly = typeof(StartupConfigurationExtensions).Assembly;

        var queryResolvers = assembly.GetTypes().Where(type =>
            type is { IsClass: true, IsAbstract: false, Namespace: not null } &&
            type.Namespace.StartsWith("Task_Flow_Manager_Extension.Resolvers") &&
            type.GetCustomAttributes(typeof(ExtendObjectTypeAttribute), true)
                .Any(attr => ((ExtendObjectTypeAttribute)attr).Name == "Query")
        );

        var mutationResolvers = assembly.GetTypes().Where(type =>
            type is { IsClass: true, IsAbstract: false, Namespace: not null } &&
            type.Namespace.StartsWith("Task_Flow_Manager_Extension.Resolvers") &&
            type.GetCustomAttributes(typeof(ExtendObjectTypeAttribute), true)
                .Any(attr => ((ExtendObjectTypeAttribute)attr).Name == "Mutation")
        );

        var schemaBuilder = builder.Services.AddGraphQLServer()
            .AddQueryType(objectTypeDescriptor => objectTypeDescriptor.Name("Query"))
            .AddMutationType(objectTypeDescriptor => objectTypeDescriptor.Name("Mutation"));

        foreach (var resolver in queryResolvers)
        {
            schemaBuilder.AddTypeExtension(resolver);
        }

        foreach (var resolver in mutationResolvers)
        {
            schemaBuilder.AddTypeExtension(resolver);
        }
    }

    private static void ConfigureAppSettings(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<SchemaSettings>(builder.Configuration.GetSection("SchemaSettings"));
    }
    
    private static void ConfigureRedis(this WebApplicationBuilder builder)
    {
        var redisConnection = builder.Configuration.GetConnectionString("Redis");
        if (string.IsNullOrWhiteSpace(redisConnection))
            throw new InvalidOperationException("Redis connection string is not configured.");

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection;
            options.InstanceName = "TaskFlow_";
        });
    }

}