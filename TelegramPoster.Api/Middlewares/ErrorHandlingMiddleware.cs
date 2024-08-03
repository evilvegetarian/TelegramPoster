using Microsoft.AspNetCore.Mvc.Infrastructure;
using TelegramPoster.Application;


namespace TelegramPoster.Api.Middlewares;

public class ErrorHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        ProblemDetailsFactory problemDetailsFactory)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (Exception exception)
        {
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
