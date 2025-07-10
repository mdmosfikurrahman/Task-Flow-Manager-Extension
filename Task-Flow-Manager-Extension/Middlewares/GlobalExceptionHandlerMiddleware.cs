using System.Net;
using System.Text.Json;
using Task_Flow_Manager_Extension.Infrastructure.Dto;
using Task_Flow_Manager_Extension.Exceptions;

namespace Task_Flow_Manager_Extension.Middlewares;

public class GlobalExceptionHandlerMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleGlobalExceptionAsync(context, ex);
        }
    }

    private Task HandleGlobalExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        string message = exception.Message;
        List<ErrorDetails> errors;

        switch (exception)
        {
            case NotFoundException e:
                statusCode = HttpStatusCode.NotFound;
                message = e.Message;
                errors = [new("Not Found", e.Message)];
                break;

            case FeatureNotImplementedException e:
                statusCode = HttpStatusCode.NotImplemented;
                message = e.Message;
                errors = [new("Not Implemented", e.Message)];
                break;

            case ValidationException e:
                statusCode = HttpStatusCode.BadRequest;
                message = e.Message;
                errors = e.Errors;
                break;

            case InactiveResourceException e:
                statusCode = HttpStatusCode.BadRequest;
                message = e.Message;
                errors = [new("Inactive Resource", e.Message)];
                break;

            case DeletedResourceException e:
                statusCode = HttpStatusCode.BadRequest;
                message = e.Message;
                errors = [new("Resource Deleted", e.Message)];
                break;

            case AlreadyExistsException e:
                statusCode = HttpStatusCode.BadRequest;
                message = e.Message;
                errors = [new ErrorDetails("Resource Exists Already", e.Message)];
                break;

            default:
                errors = [new("Internal Error", exception.Message)];
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = RestResponse<object>.Error((int)statusCode, message, errors);
        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}