using AutoMapper;

namespace Task_Flow_Manager_Extension.Mappers;

/// <summary>
/// Provides a centralized static utility for semantic and type-safe object transformation using AutoMapper.
/// Designed to streamline mapping across application layers, including models, entities, request/response DTOs,
/// partial updates, and collections. Must be configured once at startup via <see cref="Configure(IMapper)"/>.
/// </summary>
public static class MapperExtensions
{
    private static IMapper? _mapper;

    /// <summary>
    /// Initializes the internal AutoMapper instance used by all extension methods.
    /// This should be called once during application startup, typically in the Program.cs or Startup.cs.
    /// </summary>
    /// <param name="mapper">The configured <see cref="IMapper"/> instance.</param>
    public static void Configure(IMapper mapper)
    {
        _mapper = mapper;
    }

    /// <summary>
    /// Ensures that the AutoMapper instance has been properly initialized.
    /// Throws an <see cref="InvalidOperationException"/> if not configured.
    /// </summary>
    private static void EnsureConfigured()
    {
        if (_mapper == null)
            throw new InvalidOperationException("MapperExtensions is not configured. Call Configure(mapper) before using mapping extensions.");
    }

    /// <summary>
    /// Generic internal method to perform the actual transformation from a source object to a destination object.
    /// Used by all public wrapper methods for concise and centralized mapping logic.
    /// </summary>
    /// <typeparam name="TSource">The type of the input object.</typeparam>
    /// <typeparam name="TDest">The type of the resulting mapped object.</typeparam>
    /// <param name="source">The object to transform.</param>
    /// <returns>A new instance of <typeparamref name="TDest"/> mapped from <paramref name="source"/>.</returns>
    private static TDest To<TSource, TDest>(this TSource source)
    {
        EnsureConfigured();
        return _mapper!.Map<TDest>(source!);
    }

    // === SINGLE OBJECT MAPPERS ===

    /// <summary>
    /// Transforms a domain model or internal object to a standard response DTO.
    /// Commonly used to return data from services or APIs in a structured, client-friendly format.
    /// </summary>
    /// <typeparam name="TModel">The source object type representing internal structure.</typeparam>
    /// <typeparam name="TResponse">The destination type representing the external-facing response.</typeparam>
    /// <param name="model">The source object to map from.</param>
    /// <returns>The mapped response DTO.</returns>
    public static TResponse ToResponse<TModel, TResponse>(this TModel model)
        => model.To<TModel, TResponse>();

    /// <summary>
    /// Transforms a domain model or entity to a minimal or lightweight response DTO.
    /// Used in cases where partial or summarized data is sufficient (e.g., listings or previews).
    /// </summary>
    /// <typeparam name="TModel">The source object type.</typeparam>
    /// <typeparam name="TMinimalResponse">The target minimal DTO type.</typeparam>
    /// <param name="model">The object to transform.</param>
    /// <returns>A lightweight version of the response DTO.</returns>
    public static TMinimalResponse ToMinimalResponse<TModel, TMinimalResponse>(this TModel model)
        => model.To<TModel, TMinimalResponse>();

    /// <summary>
    /// Maps a given object (typically a model or response DTO) to a request DTO.
    /// Useful for pre-filling forms, resubmissions, or rehydrating state into an input model.
    /// </summary>
    /// <typeparam name="TSource">The source type (e.g., response or model).</typeparam>
    /// <typeparam name="TRequest">The request DTO type to transform into.</typeparam>
    /// <param name="source">The object to transform.</param>
    /// <returns>A mapped request DTO.</returns>
    public static TRequest ToRequest<TSource, TRequest>(this TSource source)
        => source.To<TSource, TRequest>();

    /// <summary>
    /// Maps an input object (typically a request or response DTO) to a domain model or entity.
    /// Supports use cases such as creating or updating internal records based on external input.
    /// </summary>
    /// <typeparam name="TSource">The input DTO type.</typeparam>
    /// <typeparam name="TEntity">The destination model or entity type.</typeparam>
    /// <param name="request">The DTO to map from.</param>
    /// <returns>A new instance of <typeparamref name="TEntity"/> based on the source object.</returns>
    public static TEntity ToEntity<TSource, TEntity>(this TSource request)
        => request.To<TSource, TEntity>();

    /// <summary>
    /// Maps a model to a partial request DTO, often used for patching or updating only selected fields.
    /// Ideal when full object replacement is not required.
    /// </summary>
    /// <typeparam name="TModel">The original model type.</typeparam>
    /// <typeparam name="TPartialRequest">The partial request DTO type.</typeparam>
    /// <param name="model">The source model.</param>
    /// <returns>The mapped partial request DTO.</returns>
    public static TPartialRequest ToPartialRequest<TModel, TPartialRequest>(this TModel model)
        => model.To<TModel, TPartialRequest>();

    /// <summary>
    /// Applies values from a source object to an existing destination object.
    /// This performs an in-place update without allocating a new instance.
    /// Useful for updating tracked entities or stateful objects.
    /// </summary>
    /// <typeparam name="TSource">The type of the input object.</typeparam>
    /// <typeparam name="TDest">The type of the object to be updated.</typeparam>
    /// <param name="source">The object containing updated values.</param>
    /// <param name="destination">The target object to be modified.</param>
    public static void MapToExisting<TSource, TDest>(this TSource source, TDest destination)
    {
        EnsureConfigured();
        _mapper!.Map(source, destination);
    }

    // === COLLECTION / LIST MAPPERS ===

    /// <summary>
    /// Maps a collection of internal models to a list of response DTOs.
    /// Appropriate for returning collections to API consumers or clients.
    /// </summary>
    /// <typeparam name="TModel">The type of each item in the source collection.</typeparam>
    /// <typeparam name="TResponse">The type of each item in the resulting response list.</typeparam>
    /// <param name="models">The source collection to transform.</param>
    /// <returns>A list of response DTOs.</returns>
    public static List<TResponse> ToResponseList<TModel, TResponse>(this IEnumerable<TModel> models)
        => models.To<IEnumerable<TModel>, List<TResponse>>();

    /// <summary>
    /// Maps a collection of objects to a list of minimal response DTOs.
    /// Used to return lightweight views of data collections, often for summaries or listings.
    /// </summary>
    /// <typeparam name="TModel">The source type of each element.</typeparam>
    /// <typeparam name="TMinimalResponse">The destination lightweight DTO type.</typeparam>
    /// <param name="models">The input collection.</param>
    /// <returns>A list of minimal DTOs.</returns>
    public static List<TMinimalResponse> ToMinimalResponseList<TModel, TMinimalResponse>(this IEnumerable<TModel> models)
        => models.To<IEnumerable<TModel>, List<TMinimalResponse>>();

    /// <summary>
    /// Maps a collection of input objects to a list of request DTOs.
    /// Commonly used for bulk editing, cloning, or transforming read-only data into editable formats.
    /// </summary>
    /// <typeparam name="TSource">The source item type.</typeparam>
    /// <typeparam name="TRequest">The destination request DTO type.</typeparam>
    /// <param name="sources">The input collection.</param>
    /// <returns>A list of request DTOs.</returns>
    public static List<TRequest> ToRequestList<TSource, TRequest>(this IEnumerable<TSource> sources)
        => sources.To<IEnumerable<TSource>, List<TRequest>>();

    /// <summary>
    /// Maps a collection of DTOs (request or response) to a list of domain models or entities.
    /// Useful for batch processing, import pipelines, or bulk create/update operations.
    /// </summary>
    /// <typeparam name="TSource">The input item type.</typeparam>
    /// <typeparam name="TEntity">The target entity/model type.</typeparam>
    /// <param name="requests">The collection of input DTOs.</param>
    /// <returns>A list of mapped domain models or entities.</returns>
    public static List<TEntity> ToEntityList<TSource, TEntity>(this IEnumerable<TSource> requests)
        => requests.To<IEnumerable<TSource>, List<TEntity>>();

    /// <summary>
    /// Maps a collection of models to a list of partial request DTOs.
    /// Enables partial update operations across collections, such as bulk patches.
    /// </summary>
    /// <typeparam name="TModel">The source object type.</typeparam>
    /// <typeparam name="TPartialRequest">The partial request DTO type.</typeparam>
    /// <param name="models">The input model collection.</param>
    /// <returns>A list of partial request DTOs.</returns>
    public static List<TPartialRequest> ToPartialRequestList<TModel, TPartialRequest>(this IEnumerable<TModel> models)
        => models.To<IEnumerable<TModel>, List<TPartialRequest>>();
}
