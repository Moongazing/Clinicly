using Microsoft.AspNetCore.Http;
using Moongazing.Kernel.CrossCuttingConcerns.Exceptions.Extensions;
using Moongazing.Kernel.CrossCuttingConcerns.Exceptions.HttpProblemDetails;
using Moongazing.Kernel.CrossCuttingConcerns.Exceptions.Types;
using TimeoutException = Moongazing.Kernel.CrossCuttingConcerns.Exceptions.Types.TimeoutException;

namespace Moongazing.Kernel.CrossCuttingConcerns.Exceptions.Handlers;

public class HttpExceptionHandler : ExceptionHandler
{
    public HttpResponse Response
    {
        get => response ?? throw new NullReferenceException(nameof(response));
        set => response = value;
    }

    private HttpResponse? response;

    public override Task HandleException(TimeoutException timeOutException)
    {
        Response.StatusCode = StatusCodes.Status408RequestTimeout;
        string details = new RateLimitProblemDetails(timeOutException.Message).ToJson();
        return Response.WriteAsync(details);
    }
    public override Task HandleException(RateLimitRejectedException rateLimitRejectedException)
    {
        Response.StatusCode = StatusCodes.Status429TooManyRequests;
        string details = new RateLimitProblemDetails(rateLimitRejectedException.Message).ToJson();
        return Response.WriteAsync(details);
    }
    public override Task HandleException(BusinessException businessException)
    {
        Response.StatusCode = StatusCodes.Status400BadRequest;
        string details = new BusinessProblemDetails(businessException.Message).ToJson();
        return Response.WriteAsync(details);
    }
    public override Task HandleException(ValidationException validationException)
    {
        Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
        string details = new ValidationProblemDetails(validationException.Errors).ToJson();
        return Response.WriteAsync(details);
    }
    public override Task HandleException(AuthorizationException authorizationException)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        string details = new AuthorizationProblemDetails(authorizationException.Message).ToJson();
        return Response.WriteAsync(details);
    }
    public override Task HandleException(NotFoundException notFoundException)
    {
        Response.StatusCode = StatusCodes.Status404NotFound;
        string details = new NotFoundProblemDetails(notFoundException.Message).ToJson();
        return Response.WriteAsync(details);
    }
    public override Task HandleException(Exception exception)
    {
        Response.StatusCode = StatusCodes.Status500InternalServerError;
        string details = new InternalServerErrorProblemDetails(exception.Message).ToJson();
        return Response.WriteAsync(details);
    }
}