using System.Reflection;
using AutoMapper;

namespace Task_Flow_Manager_Extension.Mappers.Profiles;

/// <summary>
/// Defines an AutoMapper profile that establishes conventional mappings between 
/// domain model classes and their corresponding Data Transfer Objects (DTOs).
/// </summary>
/// <remarks>
/// <para>
/// This profile automatically scans loaded assemblies to locate types defined within the 
/// following namespaces:
/// </para>
/// <list type="bullet">
///   <item>
///     <description><c>Task_Flow_Manager_Extension.Models</c> – Domain model classes</description>
///   </item>
///   <item>
///     <description><c>Task_Flow_Manager_Extension.Dto.Request</c> – DTOs used for client request payloads</description>
///   </item>
///   <item>
///     <description><c>Task_Flow_Manager_Extension.Dto.Response</c> – DTOs used for API response payloads</description>
///   </item>
/// </list>
/// <para>
/// Once identified, it configures mappings between the following type pairs:
/// </para>
/// <list type="bullet">
///   <item>
///     <description>Model ↔ Response DTO (e.g., <c>Partner</c> ↔ <c>PartnerResponse</c>)</description>
///   </item>
///   <item>
///     <description>Model ↔ Request DTO (e.g., <c>Partner</c> ↔ <c>PartnerRequest</c>)</description>
///   </item>
///   <item>
///     <description>Response DTO ↔ Request DTO (e.g., <c>PartnerResponse</c> ↔ <c>PartnerRequest</c>)</description>
///   </item>
/// </list>
/// </remarks>
public class ConventionMappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConventionMappingProfile"/> class
    /// and applies conventional type mappings using AutoMapper.
    /// </summary>
    public ConventionMappingProfile()
    {
        const string modelNamespace = "Task_Flow_Manager_Extension.Models";
        const string requestNamespace = "Task_Flow_Manager_Extension.Dto.Request";
        const string responseNamespace = "Task_Flow_Manager_Extension.Dto.Response";

        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

        var modelAssembly = FindAssemblyByNamespace(loadedAssemblies, modelNamespace);
        var requestAssembly = FindAssemblyByNamespace(loadedAssemblies, requestNamespace);
        var responseAssembly = FindAssemblyByNamespace(loadedAssemblies, responseNamespace);

        // If any required assembly is missing, exit early
        if (modelAssembly == null || requestAssembly == null || responseAssembly == null)
            return;

        // Retrieve all class types in the respective namespaces
        var modelTypes = modelAssembly.GetTypes()
            .Where(type => type is { IsClass: true, Namespace: modelNamespace })
            .ToList();

        var requestDtoTypes = requestAssembly.GetTypes()
            .Where(type => type is { IsClass: true, Namespace: requestNamespace })
            .ToList();

        var responseDtoTypes = responseAssembly.GetTypes()
            .Where(type => type is { IsClass: true, Namespace: responseNamespace })
            .ToList();

        // === Model ↔ Response DTO mapping
        // Maps model classes to their matching response DTOs based on class name prefix
        foreach (var modelType in modelTypes)
        {
            foreach (var responseType in responseDtoTypes
                         .Where(type => type.Name.StartsWith(modelType.Name)))
            {
                CreateMap(modelType, responseType).ReverseMap();
            }
        }

        // === Model ↔ Request DTO mapping
        // Maps model classes to their matching request DTOs based on class name prefix
        foreach (var modelType in modelTypes)
        {
            foreach (var requestType in requestDtoTypes
                         .Where(type => type.Name.StartsWith(modelType.Name)))
            {
                CreateMap(modelType, requestType).ReverseMap();
            }
        }

        // === Response ↔ Request DTO mapping
        // Maps response DTOs to request DTOs when their names match conventionally
        foreach (var responseType in responseDtoTypes)
        {
            foreach (var requestType in requestDtoTypes
                         .Where(type => type.Name == responseType.Name.Replace("Response", "Request")))
            {
                CreateMap(responseType, requestType).ReverseMap();
            }
        }
    }

    /// <summary>
    /// Searches the given assemblies for the first one that contains any type
    /// defined under the specified namespace.
    /// </summary>
    /// <param name="assemblies">The list of assemblies to search.</param>
    /// <param name="targetNamespace">The namespace to match against.</param>
    /// <returns>
    /// The <see cref="Assembly"/> containing the target namespace, or <c>null</c> if none found.
    /// </returns>
    private static Assembly? FindAssemblyByNamespace(IEnumerable<Assembly> assemblies, string targetNamespace)
    {
        return assemblies.FirstOrDefault(assembly =>
            assembly.GetTypes().Any(type => type.Namespace == targetNamespace));
    }
}
