namespace Task_Flow_Manager_Extension.Middlewares;

public class BlockDirectSchemaAccessMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        var isInternalRequest = context.Request.Headers.TryGetValue("X-Internal-Request", out var header) &&
                                string.Equals(header, "true", StringComparison.OrdinalIgnoreCase);

        if (context.Request.Path.Equals("/api/v1/graphql/schema", StringComparison.OrdinalIgnoreCase) &&
            !isInternalRequest)
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Access to raw schema is blocked.");
            return;
        }

        await next(context);
    }
}