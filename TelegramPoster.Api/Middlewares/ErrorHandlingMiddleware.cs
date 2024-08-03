using Microsoft.AspNetCore.Mvc.Infrastructure;
using TelegramPoster.Application;


namespace TelegramPoster.Api.Middlewares;

public class ErrorHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        ProblemDetailsFactory problemDetailsFactory,
        ILogger<ErrorHandlingMiddleware> logger)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (Exception exception)
        {
            logger.LogError(
                 exception,
                 "Error has happened with {RequestPath}, the message is {ErrorMessage}",
                 context.Request.Path.Value, exception.Message);

            var problemDetails = exception switch
            {
                BadRequestException badRequestException =>
                problemDetailsFactory.CreateProblemDetails(
                    context,
                    StatusCodes.Status400BadRequest,
                    badRequestException.Message),

                _ => problemDetailsFactory.CreateProblemDetails(
                                        context,
                                        StatusCodes.Status500InternalServerError,
                                        "Unhandled error! Please contact us."),
            };
            context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(problemDetails, problemDetails.GetType());
        }
    }
}